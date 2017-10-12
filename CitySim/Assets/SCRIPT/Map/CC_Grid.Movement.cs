using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid  {



    Vector2 interpBetweenTwo(float y, float y1, float y2, Vector2 V1, Vector2 V2)
    {
        Vector2 vel1 = V1 * (y2 - y) / (y2 - y1);
        Vector2 vel2 = V2 * (y - y1) / (y2 - y1);
        return vel1 + vel2;
    }

    Vector2 interpBetweenFour(float x, float y, float x1, float y1,
        float x2, float y2, Vector2 V1, Vector2 V2, Vector2 V3, Vector2 V4)
    {
        float mult1 = (x2 - x) / (x2 - x1);
        float mult2 = (x - x1) / (x2 - x1);

        Vector2 vel1 = V1 * mult1 + V3 * mult2;
        Vector2 vel2 = V2 * mult1 + V4 * mult2;

        Vector2 final = vel1 * ((y2 - y) / (y2 - y1)) + vel2 * ((y-y1) / (y2-y1));
        return final;
    }

    void Movement(int Group)
    {
        MV_Type type = MovementManager.Instance.MoveType;

        if (type != MV_Type.ContinuumCrowd)
            return;

        CC_GroupGrid group = GroupGridList[Group];
        foreach (Rigidbody body in AgentBodys)
        {
            Vector2 pos = body.transform.position;
            Vector2 cell = Utility.Instance.Vec3ToCoord(pos);
            Vector2 finalVelocity = Vector2.zero;

            int x = (int)cell.x;
            int y = (int)cell.y;

            bool a = true, c = false, b = false, d = false;

            Vector2 cellA, cellB, cellC, cellD;
            Vector2 velA = Vector2.zero,
                velB = Vector2.zero,
                velC = Vector2.zero,
                velD = Vector2.zero;
            //velA = group.Grid[x, y].TotalVelocity;

            

            cellA = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y));
            cellB = new Vector2(Mathf.Ceil(pos.x), Mathf.Floor(pos.y));
            cellC = new Vector2(Mathf.Ceil(pos.x), Mathf.Ceil(pos.y));
            cellD = new Vector2(Mathf.Floor(pos.x), Mathf.Ceil(pos.y));

            Vector2 coord;
            if (Utility.Instance.Vec2ToCoordExists(cellA, out coord))
            {
                int ax = (int)coord.x;
                int ay = (int)coord.y;
                Vector2 Apos = group.Grid[ax, ay].WorldPos;
                if (pos == Apos)
                {
                    body.velocity = velA;
                    continue;
                }
                velA = group.Grid[ax, ay].TotalVelocity;
                a = true;

            }
            if (Utility.Instance.Vec2ToCoordExists(cellB, out coord))
            {
                int bx = (int)coord.x;
                int by = (int)coord.y;
                velB = group.Grid[bx, by].TotalVelocity;
                b = true;
                
            }
            if (Utility.Instance.Vec2ToCoordExists(cellC, out coord))
            {
                int cx = (int)coord.x;
                int cy = (int)coord.y;
                velC = group.Grid[cx, cy].TotalVelocity;
                c = true;
            }
            if (Utility.Instance.Vec2ToCoordExists(cellD, out coord))
            {
                int dx = (int)coord.x;
                int dy = (int)coord.y;
                velD = group.Grid[dx, dy].TotalVelocity;
                d = true;
            }

            

            float x1 = cellA.x;
            float x2 = cellC.x;
            float y1 = cellA.y;
            float y2 = cellC.y;
            float xf = pos.x;
            float yf = pos.y;

            float diffx = x2 - x1;
            float diffy = y2 - y1;

            if (diffx == 0.0f)
                finalVelocity = interpBetweenTwo(y, y1, y2, velB, velC);
            else if (a == false && d == false)
            {
                finalVelocity = interpBetweenTwo(y, y1, y2, velB, velC);
                finalVelocity.x = Mathf.Min(finalVelocity.x, 0f);
            }
            else if (c == false && b == false)
            {
                finalVelocity = interpBetweenTwo(y, y1, y2, velA, velD);
                finalVelocity.x = Mathf.Min(finalVelocity.x, 0f);
            }
            else if (diffy == 0.0f)
                finalVelocity = interpBetweenTwo(x, x1, x2, velD, velC);
            else if (a == false && b == false)
            {
                finalVelocity = interpBetweenTwo(x, x1, x2, velD, velC);
                finalVelocity.y = Mathf.Min(finalVelocity.y, 0.0f);
            }
            else if (c == false && d == false)
            {
                finalVelocity = interpBetweenTwo(x, x1, x2, velA, velB);
                finalVelocity.y = Mathf.Min(finalVelocity.y, 0.0f);
            }
            else
                finalVelocity = interpBetweenFour(x, y, x1, y1, x2, y2, velA, velB, velD, velC);

            body.velocity = finalVelocity;// + new Vector2(body.velocity.x, body.velocity.y);
        }
    }
}
