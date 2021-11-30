using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCollisionInfo : MonoBehaviour
{

    Collider physicsObject;
    public bool collision;

    // Start is called before the first frame update
    void Start()
    {
        physicsObject = GetComponent<Collider>();
        physicsObject.isTrigger = true;
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
