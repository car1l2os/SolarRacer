using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterSeconds : MonoBehaviour {

    [SerializeField]
    float _secondsToDelete = 1f;
	
	// Update is called once per frame
	void Update () {
        _secondsToDelete -= Time.deltaTime;

        if (_secondsToDelete <= 0f)
            Destroy(this.gameObject);
	}
}
