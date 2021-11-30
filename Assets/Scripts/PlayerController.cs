using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerController : MonoBehaviour {
    private List<AxleInfo> axleInfos;
    private AudioSource carMotorSound, boostSound, noBoostLeftSound, brakeSound, honkSound;
    private CarStats cs;
    private UIManager UIM;
    private PlayerInput pi;
    public Rigidbody rb;        // Used by PlayerUiScript
    public Gamepad gp;          // Set by Instantiateplayer
    public GameObject playerUI; // Used by Instatiateplayer
    public int playerId;        // Used by Instatiateplayer
    private bool isBoosting, boostEmptySound;
    private float maxMotorTorque, maxSteeringAngle, maxBreakForce, maxSpeed, steering, motor, brakes, volume, speed, lastBoostTime, lastResetTime, boostRegen, boostDuration, noBoostTime;
    public float boost;         // Used by ResetBoost
	private GameInfo gameInfo;

    void Start() {
        UIM = GameObject.Find("ClockAndPausMenu").transform.Find("UIManager").gameObject.GetComponent<UIManager>();
        gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();
        cs = GetComponent<CarStats>();
        carMotorSound = GetComponent<AudioSource>();
        boostSound = GameObject.Find("BoostSound").GetComponent<AudioSource>();
        noBoostLeftSound = GameObject.Find("NoBoostSound").GetComponent<AudioSource>();
        boostSound.volume = 0.7F;
        noBoostLeftSound.volume = 0.7F;
        brakeSound = GameObject.Find("BrakeSound").GetComponent<AudioSource>();
        honkSound = GameObject.Find("HonkSound").GetComponent<AudioSource>();
        maxMotorTorque = cs.maxMotorTorque;
        maxSteeringAngle = cs.maxSteeringAngle;
        maxBreakForce = cs.maxBreakForce;
        maxSpeed = cs.maxSpeed;
        axleInfos = cs.axleInfos;
        boostRegen = cs.boostRegen;
        boostDuration = cs.boostDuration;
        rb = GetComponent<Rigidbody>();
        rb.mass = cs.mass;
        pi = GetComponent<PlayerInput>();
        pi.neverAutoSwitchControlSchemes = true;
        lastBoostTime = -6F;
        lastResetTime = -3F;
        rb.centerOfMass = cs.centerOfMass;
        boost = 1f;
        isBoosting = false;
        boostEmptySound = false;

        if(!gameInfo.enableEffects) {
            carMotorSound.Stop();
        }
    }

    void Update() {
        if(gameInfo.enableEffects) {
            MotorSound();
        }
    }

    void FixedUpdate() {
        if(UIM.gameStarted) {
            HandleSteeringAndMotor();
            BoostHandler();
        }
	}

    /* Player input controlled functions -------------------------------------------------------- */
    public void OnMove(InputValue value) {
        if(pi.currentControlScheme == "Gamepad" || pi.currentControlScheme == "Gamepad2") {
            steering = maxSteeringAngle * gp.leftStick.ReadValue().x;
        }
        else {
            steering = maxSteeringAngle * (float)value.Get();
        }
    }

    public void OnThrottle() {
        if(pi.currentControlScheme == "Gamepad" || pi.currentControlScheme == "Gamepad2") {
            if(gp.rightTrigger.ReadValue() > 0.2) {
                motor = maxMotorTorque * gp.rightTrigger.ReadValue();
            }
            else {
                motor = 0;
            }
        }
        else if(pi.currentControlScheme == "WASD") {
            motor = maxMotorTorque;
        }
        else if(pi.currentControlScheme == "Arrows"){
            motor = maxMotorTorque;
        }
        else {
            motor = 0;
        }
    }

    // Throttle button is released
    public void OnNoThrottle() {
        motor = 0;
    }

    public void OnBrake(InputValue value) {
        playBrakeSound();
        if(pi.currentControlScheme == "Gamepad" || pi.currentControlScheme == "Gamepad2") {
            if(gp.buttonWest.ReadValue() == 1) {
                brakes = maxBreakForce * gp.buttonWest.ReadValue();
            }
            else {
                stopBrakeSound();
                brakes = 0;
            }
        }
        else if(pi.currentControlScheme == "WASD" || pi.currentControlScheme == "Arrows") {
            brakes = maxBreakForce;
        }
    }

    // Brake button is released
    public void OnNoBrake() {
        stopBrakeSound();
        brakes = 0;
    }

    public void OnReverse() {
        if(pi.currentControlScheme == "Gamepad" || pi.currentControlScheme == "Gamepad2") {
            if(gp.leftTrigger.ReadValue() > 0.2) {
                maxSpeed = cs.maxSpeed / 2; // Cannot reverse as fast as going forward
                motor = -1 * maxMotorTorque * gp.leftTrigger.ReadValue();
            }
            else {
                maxSpeed = cs.maxSpeed;
                motor = 0;
            }
        }
        else if(pi.currentControlScheme == "WASD" || pi.currentControlScheme == "Arrows") {
            maxSpeed = cs.maxSpeed / 2; // Cannot reverse as fast as going forward
            motor = -1 * maxMotorTorque;
        }
    }

    // Reverse button is released
    public void OnNoReverse() {
        maxSpeed = cs.maxSpeed;
        motor = 0;
    }

    public void OnBoost() {
        if(UIM.gameStarted) {
            if(!isBoosting && boost>0) {
                boostEmptySound = true;
                isBoosting = true;
                playBoostSound(); 
                maxSpeed = cs.maxSpeed * (1 + cs.maxMotorTorque / 2000) ;
            } else if (!isBoosting && boost <= 0){
                if(boostEmptySound) {
                    playNoBoostLeftSound();
                    boostEmptySound = false;
                }
            } 
        }
    }

    // Boost button is released
	public void OnNoBoost() {
        if(UIM.gameStarted) {
            stopBoostSound();
            lastBoostTime = UIM.TimePlayed;
            isBoosting = false;
            maxSpeed = cs.maxSpeed;
            boostEmptySound = true;
        }
    }
  
	public void OnHonk() {
        if(gameInfo.enableEffects) {
		    PlayHonkSound();

        }
    }

	public void OnPause(){
		UIM.OnPause();
	}

    // This function resets the car to an upright position 
    public void OnReset(){
        if(UIM.gameStarted) {
            /* Can only reset every 3 seconds at max. Can not reset when moving faster than 6 m/s (and not when paused) */
            if (lastResetTime + 3F < UIM.TimePlayed && Time.timeScale != 0 && rb.velocity.magnitude <= 6F) {
                lastResetTime = UIM.TimePlayed;
                this.gameObject.transform.position =  new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1, this.gameObject.transform.position.z); 
                this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, 0);
                rb.velocity = new Vector3(0,0,0);
                rb.angularVelocity = new Vector3(0,0,0);
            }
        }
	}

    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void HandleSteeringAndMotor() {
        speed = rb.velocity.magnitude;
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                if(speed < maxSpeed) {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                } else {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                }
            }
            axleInfo.leftWheel.brakeTorque = brakes;
            axleInfo.rightWheel.brakeTorque = brakes;
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    /* This function handles the regeneration of boost and the boosts effect */
    public void BoostHandler() {
        if(!isBoosting) {
            if(boost < 1f) {
                if(UIM.TimePlayed-lastBoostTime > 2f) {
                    boost += Time.deltaTime/boostRegen;
                }
            } else {
                boost = 1f;
            }
        } else {
            if(boost > 0) {
                // Increase velocity until at new maxSpeed
                if(rb.velocity.magnitude <= maxSpeed) {
                    rb.velocity +=  ((1 + cs.maxMotorTorque / 2000) / 5 * transform.forward) * boost;
                }
                boost -= Time.deltaTime/boostDuration;
            } else {
                boost = 0;
                maxSpeed = cs.maxSpeed;
                if(boostEmptySound) {
                    playNoBoostLeftSound();
                    boostEmptySound = false;
                }
            }
        }
    }


    /* Functions handling sound ------------------------------------------------------------------- */
    /* Changes sound of motor (and brakes) depending upon speed of the car */
    public void MotorSound() {
      if(Time.timeScale == 1) {
        carMotorSound.pitch =Mathf.Clamp(0.3f + Math.Abs(motor/maxMotorTorque)*0.5f + speed/30f, 0.3f, 3f);
        carMotorSound.volume = Mathf.Clamp(0.3F + Math.Abs(speed)/100 + Math.Abs(motor/maxMotorTorque)*0.2f, 0.3f, 1f);
        if (Math.Abs(speed) < 0.5F) { // Speed is almost never exactly 0
	    stopBrakeSound();
        }
        else {
      		brakeSound.volume = Math.Abs(speed)/20;
        }
      }
      else {
        carMotorSound.volume = 0;
        carMotorSound.pitch = 0;
      }
    }

	public void stopBoostSound() {
		boostSound.Stop(); 
	}

	public void playBoostSound() {
        if(gameInfo.enableEffects) {
		    boostSound.Play(); 
        }
	}

	public void playNoBoostLeftSound() {
        stopBoostSound();
        if(gameInfo.enableEffects) {
		    noBoostLeftSound.Play(); 
        }
	}

    public void playBrakeSound() {
		if(speed > 4 && gameInfo.enableEffects && (axleInfos[0].leftWheel.isGrounded || axleInfos[0].rightWheel.isGrounded || axleInfos[1].leftWheel.isGrounded || axleInfos[1].rightWheel.isGrounded)) {
			brakeSound.Play(); 
		}
 	}

    public void stopBrakeSound() {
        brakeSound.Stop(); 
    }

	public void PlayHonkSound() {
        if(!honkSound.isPlaying && gameInfo.enableEffects) {
            honkSound.Play(); 
        }
        /* To turn of sirens */
        else {
            honkSound.Stop(); 
        }
    }
}
