using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel, rightWheel;
    public bool motor, steering, brakes;
}


public class CarStats : MonoBehaviour {
    public float maxMotorTorque, maxSteeringAngle, maxBreakForce, maxSpeed, boostRegen, boostDuration, mass;
    public AxleInfo frontAxle, rearAxle;
    public List<AxleInfo> axleInfos;
    public  Vector3 centerOfMass;
    

    void Start() {
      frontAxle.leftWheel = GetComponentsInChildren<WheelCollider>()[0];
      frontAxle.rightWheel = GetComponentsInChildren<WheelCollider>()[2];
      rearAxle.leftWheel = GetComponentsInChildren<WheelCollider>()[1];
      rearAxle.rightWheel = GetComponentsInChildren<WheelCollider>()[3];
      axleInfos.Add(frontAxle);
      axleInfos.Add(rearAxle);
    }
}
