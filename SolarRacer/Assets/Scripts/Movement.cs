using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour {

    [SerializeField]
    private float _speed = 10;
    Vector3 tempPos = Vector3.zero;
	
    public float Speed
    {
        set
        {
            _speed = value;
        }
    }

	// Update is called once per frame
	void Update () {
        tempPos = transform.position;
        tempPos.x += _speed * Time.deltaTime;

        transform.position = tempPos;
	}
}
