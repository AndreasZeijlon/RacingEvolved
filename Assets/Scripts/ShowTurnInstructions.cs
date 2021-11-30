using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTurnInstructions : MonoBehaviour
{
    Collider turn;
    public bool collision;

    // Start is called before the first frame update
    void Start()
    {
        turn = GetComponent<Collider>();
        turn.isTrigger = true;
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
