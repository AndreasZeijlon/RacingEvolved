using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResetBoost : MonoBehaviour {

    Collider nitro;
    private Vector3 rotatevector;
    AudioSource nitroPickup;

      // Start is called before the first frame update
    void Start() {
        nitro = GetComponent<Collider>();
        nitro.isTrigger = true;
        nitroPickup = gameObject.transform.parent.gameObject.GetComponent<AudioSource>();
    }
        
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            PlayPickUpSound();
            other.gameObject.GetComponent<PlayerController>().boost = 1F;
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate() {
        this.gameObject.transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime, Space.World);
    }

    void PlayPickUpSound() {
        nitroPickup.Play();
    }
}
