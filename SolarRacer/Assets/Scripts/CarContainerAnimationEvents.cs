using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarContainerAnimationEvents : MonoBehaviour {

    VehicleManager _vechicleManager;

    private void Start()
    {
        _vechicleManager = GetComponentInParent<VehicleManager>();
    }

    void StartFly()
    {
        _vechicleManager.CanChangeRoad = false;
        _vechicleManager.FlyVelocity();
        //GetComponentInParent<BoxCollider>().enabled = false;
    }

    void TouchGroundAfterFly()
    {
        _vechicleManager.CanChangeRoad = true;
        _vechicleManager.GroundVelocity();
        //GetComponentInParent<BoxCollider>().enabled = true;

    }
}
