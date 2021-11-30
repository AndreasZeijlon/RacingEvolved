using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour {
    Collider stop_box;
    public bool finished;
    public UIManager UIM;

    void Start() {
        UIM = GameObject.Find("UIManager").GetComponent<UIManager>();
        stop_box = GetComponent<Collider>();
        stop_box.isTrigger = true;
        finished = false;
    }
    /* Check for collision with player and finish line  */
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            finished = true;
            UIM.CrossedGoalLine(other);
        }
    }
}
