﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBreakInstructions : MonoBehaviour
{
    Collider brake;
    public bool collision;

    // Start is called before the first frame update
    void Start()
    {
        brake = GetComponent<Collider>();
        brake.isTrigger = true;
        collision = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            collision = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            collision = false;
        }
    }
}
