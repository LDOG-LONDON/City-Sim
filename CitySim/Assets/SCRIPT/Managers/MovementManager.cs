using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MV_Type
{
    NONE,
    SimpleNav,
    Flocking,
    ContinuumCrowd
};

public class MovementManager : Singleton<MovementManager> {

    protected MovementManager() {}
    public MV_Type MoveType = MV_Type.ContinuumCrowd;

    public bool UseSimpleFlocking = true;
    public bool UseSimpleNavigation;
    public bool UseContinuumCrowd;
    public bool UseAStar = true;
    public bool UseRubberbanding = true;

}
