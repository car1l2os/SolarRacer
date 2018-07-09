using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    public float speed = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 tempPos = transform.position;

        tempPos.x += speed;

        transform.position = tempPos;

        if (transform.position.x > 30)
        {
            Destroy(gameObject);
        }
	}
}
