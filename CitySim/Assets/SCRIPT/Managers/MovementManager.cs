using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager> {

    protected MovementManager() {}

    public bool UseSimpleFlocking = true;
    public bool UseSimpleNavigation;
    public bool UseContinuumCrowd;
    public bool UseAStar = true;

}
