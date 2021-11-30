using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brake : MonoBehaviour
{

    public WheelCollider frontLeft, rightLeft, rearLeft, rearRight;

    // Start is called before the first frame update
    void Start()
    {
        frontLeft.brakeTorque = 10;
        rightLeft.brakeTorque = 10;
        rearLeft.brakeTorque = 10;
        rearRight.brakeTorque = 10;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
