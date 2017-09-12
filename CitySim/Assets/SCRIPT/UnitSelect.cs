using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelect : MonoBehaviour {

    public bool isSelected;

	public void OnSelect()
    {
        isSelected = true;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void OnUnselect()
    {
        isSelected = false;
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
