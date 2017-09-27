using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CC_Grid {

    void DensityConversion()
    {
        foreach (CC_AgentData agent in AgentData)
        {
            // not sure if its talking about cell coord or world coords

            // Grid Setup
            //  D | C
            // -------
            //  A | B

            Vector2 agentVelocity = agent.GetComponent<Rigidbody>().velocity;
            Vector3 agentWorld = agent.transform.position;
            Vector3 coord = Utility.Instance.Vec3ToCoord(agentWorld);
            int Cx = (int)coord.x;
            int Cy = (int)coord.y;

            int Dx = Cx;
            int Dy = Cy;
            int Bx = Cx;
            int By = Cy;
            int Ax = Cx;
            int Ay = Cy;
            bool dontUseDA = false;
            bool dontUseBA = false;

            // check if 0 on x axis
            if (Cx <= 0)
            {
                Dx = 0;
                Ax = 0;
                dontUseDA = true;
            }
            else
            {
                Dx--;
                Ax--;
            }
            //check if 0 on y axis
            if (Cy <= 0)
            {
                By = 0;
                Ay = 0;
                dontUseBA = true;
            }
            else
            {
                By--;
                Ay--;
            }

            float invX;
            float invY;

            float densityC = 0f;
            float densityA = 0f;
            float densityB = 0f;
            float densityD = 0f;

            Vector3 cWorld = GlobalGrid[Cx, Cy].WorldPos;
            Vector3 dWorld = GlobalGrid[Dx, Dy].WorldPos;
            Vector3 bWorld = GlobalGrid[Bx, By].WorldPos;
            Vector3 aWorld = GlobalGrid[Ax, Ay].WorldPos;

            Vector3 minCellWorld = GlobalGrid[Ax, Ay].WorldPos;
            float deltaX = agentWorld.x - minCellWorld.x;
            float deltaY = agentWorld.y - minCellWorld.y;


            // coord [0,0] on grid
            if (dontUseBA && dontUseDA)
            {
                // case if we use coord [-1,-1] for "min" cell
                Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, -1);
                deltaX = agentWorld.x - negMinCell.x;
                deltaY = agentWorld.y - negMinCell.y;

                densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                GlobalGrid[Cx, Cy].Density += densityC;
            }

            // case #1: agent is below (in both x & y) it current cell's center
            if (agentWorld.x <= cWorld.x && agentWorld.y <= cWorld.y)
            {
                // coord [0,y] on grid
                if (dontUseDA)
                {
                    // case if we use coord [-1,y-1] for "min" cell
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, By);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityB = Mathf.Pow(Mathf.Min(deltaX, 1f - deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Bx, By].Density += densityB;
                }
                // coord [x,0] on grid
                else if (dontUseBA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(Dx, -1);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityD = Mathf.Pow(Mathf.Min(1f - deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Dx, Dy].Density += densityD;
                }
                // coord [x,y] on grid (from the example)
                else
                {
                    Vector3 negMinCell = aWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityB = Mathf.Pow(Mathf.Min(deltaX, 1f - deltaY), Lambda); // fix
                    densityA = Mathf.Pow(Mathf.Min(1f - deltaX, 1f - deltaY), Lambda); // fix
                    densityD = Mathf.Pow(Mathf.Min(1f - deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Bx, By].Density += densityB;
                    GlobalGrid[Dx, Dy].Density += densityD;
                    GlobalGrid[Ax, Ay].Density += densityA;
                }
            }

            // case #2: agent is above cell center in x-axis but lower on the y
            else if (agentWorld.x > cWorld.x && agentWorld.y < cWorld.y)
            {
                // coord [x, 0] on gid
                if (dontUseBA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(Cx, -1);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                    GlobalGrid[Cx, Cy].Density += densityC;
                }
                // coord [x,y] on grid
                else
                {
                    Vector3 negMinCell = bWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityB = Mathf.Pow(Mathf.Min(deltaX, 1f - deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Bx, By].Density += densityB; // fix
                }
            }

            // case #3: both x & y of agent are above its current cells center
            else if (agentWorld.x > cWorld.x && agentWorld.y > cWorld.y)
            {
                Vector3 negMinCell = cWorld;
                deltaX = agentWorld.x - negMinCell.x;
                deltaY = agentWorld.y - negMinCell.y;
                densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                GlobalGrid[Cx, Cy].Density += densityC;
            }

            // case #4: agents x is less than its currents cell and its y is greater
            else
            {
                // coord [0, y] on grid
                if (dontUseDA)
                {
                    Vector3 negMinCell = Utility.Instance.CoordToVec3(-1, Cx);
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);

                    GlobalGrid[Cx, Cy].Density += densityC;
                }
                // coord [x,y] on grid
                else
                {
                    Vector3 negMinCell = dWorld;
                    deltaX = agentWorld.x - negMinCell.x;
                    deltaY = agentWorld.y - negMinCell.y;
                    densityC = Mathf.Pow(Mathf.Min(deltaX, deltaY), Lambda);
                    densityD = Mathf.Pow(Mathf.Min(1f - deltaX, deltaY), Lambda); // fix

                    GlobalGrid[Cx, Cy].Density += densityC;
                    GlobalGrid[Dx, Dy].Density += densityD;
                }
            }

            // sumation of density * velocity for each agent applied to 
            // thier corresponding cells
            // coord [0,0]
            if (dontUseDA == true && dontUseBA == true)
            {
                GlobalGrid[Cx, Cy].AverageVelocity += densityC * agentVelocity;
            }
            // coord [x,0]
            else if (dontUseBA == true)
            {
                GlobalGrid[Dx, Dy].AverageVelocity += densityD * agentVelocity;
                GlobalGrid[Cx, Cy].AverageVelocity += densityC * agentVelocity;
            }
            // coord [0,y]
            else if (dontUseDA == true)
            {
                GlobalGrid[Bx, By].AverageVelocity += densityB * agentVelocity;
                GlobalGrid[Cx, Cy].AverageVelocity += densityC * agentVelocity;
            }
            // coord [x,y]
            else
            {
                GlobalGrid[Dx, Dy].AverageVelocity += densityD * agentVelocity;
                GlobalGrid[Bx, By].AverageVelocity += densityB * agentVelocity;
                GlobalGrid[Cx, Cy].AverageVelocity += densityC * agentVelocity;
                GlobalGrid[Ax, Ay].AverageVelocity += densityA * agentVelocity;
            }
        }

        // divide the average velocity summation on each grid cell with its total density
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (GlobalGrid[x, y].Density > 0)
                    GlobalGrid[x, y].AverageVelocity = GlobalGrid[x, y].AverageVelocity / GlobalGrid[x, y].Density;
            }
        }

    }

}
