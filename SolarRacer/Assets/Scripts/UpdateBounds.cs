using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBounds : MonoBehaviour {
	
	// Update is called once per frame
	void FixedUpdate () {
        MeshFilter[] mesh  = gameObject.GetComponentsInChildren<MeshFilter>();

        foreach(MeshFilter m in mesh)
        {
            m.mesh.RecalculateBounds();
        }
    }
}
