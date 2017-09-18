using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager> {

    protected MovementManager() {}

    public bool UseSimpleFlocking = true;
    public bool UseSimpleNavigation;
    public bool UseContinuumCrowd;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
