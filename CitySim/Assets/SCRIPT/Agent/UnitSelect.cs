using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelect : MonoBehaviour {

    public bool isSelected;

    Rigidbody body;
    Vector3 oldLook;

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

    public void Start()
    {
        body = transform.GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (body.velocity.sqrMagnitude < 0.1f)
            oldLook = body.velocity.normalized;
        else
            oldLook = body.velocity.normalized;
        transform.LookAt(transform.position + oldLook,Vector3.forward);
    }
}
