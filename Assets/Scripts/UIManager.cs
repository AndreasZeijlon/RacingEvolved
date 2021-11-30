using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour {
	private GameObject pauseButtons, finishButtons, ShowInstructions, countdown, beep_sound, start_sound;
	private Text finishText, clock, instructions, star1_time, star2_time, star3_time;
	private GameInfo gameInfo;
	private RawImage star1, star2, star3;
	private AudioSource buttonSound, beepSound, startSound, finishSound;
	private bool ShowCollisionInstr = false,  ShowTurnInstr = false, ShowBreakInstr = false, ShowBoostInstr = false, ShowResetInstr = false, ShowHonkInstr = false, ShowPauseInstr = false;
	private string throttleKey, turnKeys, breakKey, boostKey, reverseKey, resetKey, pauseKey, honkKey;
	public bool gameStarted;						// Used by PlayerController
	public float TimePlayed, minutes, seconds;		// Used by PlayerController

	void Start () {
		gameStarted = false;	// Game start is after countdown
		TimePlayed = 0F;		// Start time count at 0
		Time.timeScale = 1; 	// normal time flow
		
		/* Gameobjects */
		pauseButtons = GameObject.Find("PauseButtons");
		finishButtons = GameObject.Find("FinishButtons");
		ShowInstructions = GameObject.Find("ShowInstructions");
		countdown = GameObject.Find("Countdown");
		beep_sound = GameObject.Find("Beep_sound");
		start_sound = GameObject.Find("Start_sound");

		/* Components */
		finishText = GameObject.Find("Finish").GetComponent<Text>();
		instructions = GameObject.Find("InfoText").GetComponent<Text>();
		clock = GameObject.Find("Clock").GetComponent<Text>();
		star1 = GameObject.Find("Star1").GetComponent<RawImage>();
		star2 = GameObject.Find("Star2").GetComponent<RawImage>();
		star3 = GameObject.Find("Star3").GetComponent<RawImage>();
		star1_time = GameObject.Find("Star1_time").GetComponent<Text>();
		star2_time = GameObject.Find("Star2_time").GetComponent<Text>();
		star3_time = GameObject.Find("Star3_time").GetComponent<Text>();
		gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();
	
		initAudioSources();
		clock.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height/2-50); 	// Set position of clock depending on screen size
		ShowInstructions.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height/2-150);	// Set position of instructions  depending on screen size
		setKeyStrings(gameInfo.playerControls[0]); 																	// Display keymappings for player 1
		finishText.text = ""; 																					// Should only have any text here when finished
		
		instructions.text = "To throttle, press: " + throttleKey + "\n To reverse, press: " + reverseKey; 		// First instruction to show is throttle/brake} // Only display instructions on startmap
		hideInstr();
		hidePaused();																							// Only display pauseobjects when paused
		hideFinished();																							// Only display finishobjects when finished
		StartCoroutine(Countdown(3));																			// 3 seconds countdown before game starts
	}

	void Update () {
		/* Only show instructions for first map */
		if(gameInfo.mapSelected == 1) {
			setGameInstructions();
		}
		if(gameStarted){
			/* Increase time as real time */
			TimePlayed += Time.deltaTime;
			/* Only set clock if we have started playing */
			if (TimePlayed > 0F) {
				minutes = Mathf.Floor(TimePlayed / 60);
				seconds = TimePlayed - minutes * 60;
				if(minutes == 0) {
					clock.text = seconds.ToString("0#.00");

				} else {
					clock.text = minutes.ToString("00") + ":" + seconds.ToString("0#.00");
				}
			}
		}
	}

	/* Stops/resumes time and shows/hides pausemenu - controlled by player input*/
   	public void OnPause(){
		PlayButtonSound();
		if(Time.timeScale == 1)
		{
			Time.timeScale = 0;
			showPaused();
		} else if (Time.timeScale == 0){
			Time.timeScale = 1;
			hidePaused();
		}
	}

	// Restarts current map - controlled from pausmenu by player input
	public void Reload(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	// Loads main menu - controlled from pausmenu by player input
	public void LoadMain(){
		PlayButtonSound();
		SceneManager.LoadScene(0);
	}

	/* Functions used by menu to show relevant buttons/info ------------ */
	public void showPaused(){
		pauseButtons.SetActive(true);
	}

	public void hidePaused(){
		pauseButtons.SetActive(false);
	}

	public void showFinished(){
		finishButtons.SetActive(true);
	}

	public void hideFinished(){
		finishButtons.SetActive(false);
	}

	public void showInstr(){
		ShowInstructions.SetActive(true);
	}

	public void hideInstr() {
		ShowInstructions.SetActive(false);
	}
	/* End of functions used by menu to show relevant buttons/info ------ */

	/* Functions for sounds ------------------------------------------ */
	public void PlayButtonSound() {
		if(gameInfo.enableEffects) {
        	buttonSound.Play(); 
		}
    }

	public void PlayBeepSound() {
		if(gameInfo.enableEffects) {
        	beepSound.Play(); 
		}
    }

	public void PlayStartSound() {
		if(gameInfo.enableEffects) {
        	startSound.Play(); 
		}
    }

	public void PlayFinishSound() {
		if(gameInfo.enableEffects) {
			finishSound.Play();
		}
	}

	public void initAudioSources() {
        buttonSound = GameObject.Find("PlayButtonSound").GetComponent<AudioSource>();
		beepSound = GameObject.Find("Beep_sound").GetComponent<AudioSource>();
		startSound = GameObject.Find("Start_sound").GetComponent<AudioSource>();
		finishSound = GameObject.Find("Finish_sound").GetComponent<AudioSource>();
    }
	/* End of functions for sounds --------------------------------------- */

	
	/* Retrieves what controls player is using and shows relevant instructions on startmap */
	private void setKeyStrings(int playerControls) {
		if(playerControls == 0) {
			throttleKey = "W";
			turnKeys = "A/D";
			breakKey = "Space";
			boostKey = "Left Shift";
			reverseKey = "S";
			resetKey = "R";
			honkKey = "Q";
			pauseKey = "ESC";
		}
		else if(playerControls == 1) {
			throttleKey = "Up Arrow";
			turnKeys = "Left/Right Arrow";
			breakKey = "Right Control" ;
			boostKey = "Right Shift";
			reverseKey = "Down Arrow";
			resetKey = "Enter";
			honkKey = "DEL";
			pauseKey = "Backspace";
		}
		else {
			throttleKey = "Right Trigger";
			turnKeys = "Left Stick";
			breakKey = "West Button (Square on PS4)";
			boostKey = "South Button (X on PS4)";
			reverseKey = "Left Trigger";
			resetKey = "East Button (Circle on PS4)";
			honkKey = "North Button (Triangle on PS4)";
			pauseKey = "Start (Options on PS4)";
		}
	}

	/* This functions loads the instructions - keys displayed for each action are retrieved with "setKeyStrings" */
	private void setGameInstructions() {
		/* Show instructions if you have 0 stars on startMap or if toggled on in menu */
		if((gameInfo.bestTimes[0] > gameInfo.scoreTable[0, 2] || gameInfo.enableInstructions)) {
			showInstr();
			ShowCollisionInstr = GameObject.Find("Collisions").GetComponent<ShowCollisionInfo>().collision;
			ShowTurnInstr = GameObject.Find("Turn").GetComponent<ShowTurnInstructions>().collision;
			ShowBreakInstr = GameObject.Find("Break").GetComponent<ShowBreakInstructions>().collision;
			ShowBoostInstr = GameObject.Find("Boost").GetComponent<ShowBoostInstructions>().collision;
			ShowResetInstr = GameObject.Find("Reset").GetComponent<ShowResetInstructions>().collision;
			ShowPauseInstr = GameObject.Find("Pause").GetComponent<ShowPauseInstructions>().collision;
			ShowHonkInstr = GameObject.Find("Honk").GetComponent<ShowHonkInstructions>().collision;
			/* Change instruction based on collision with invisible objects placed strategically at startmap */
			if (ShowCollisionInstr) {
				instructions.text = "Some objects can be collided with, try running one over!";
			}
			if(ShowTurnInstr) {
				instructions.text = "To turn, use: " + turnKeys;
			}
			if(ShowBreakInstr) {
				instructions.text = "To break, use: " + breakKey;
			}
			if(ShowBoostInstr) {
				instructions.text = "To boost, use: " + boostKey +".\nBoost when the blue meter is full for maximum effect";
			}
			if(ShowResetInstr) {
				instructions.text = "If you flip your car or get stuck, reset it with: " + resetKey;
			}
			if(ShowPauseInstr) {
				instructions.text = "The game can be paused at any time using: " + pauseKey;
			}
			if(ShowHonkInstr) {
				instructions.text = "To honk, use: " + honkKey;
			}
		}
		/* Otherwise hide instructions */
		else {
			hideInstr();
		}
	}

	/* This function is called when the first player reaches the goalline. Time is stopped and finishtime is shown */
	public void CrossedGoalLine(Collider player) {
		showFinished();
		PlayFinishSound();
		Time.timeScale = 0; // Pause game
		/* Overwrite saved time if beat currently best time saved */
		if(TimePlayed < gameInfo.bestTimes[gameInfo.mapSelected-1]) {
			gameInfo.bestTimes[gameInfo.mapSelected-1] = TimePlayed;
			gameInfo.SaveGame();
		}
		clock.text = ""; // Stop showing clock time when finished - instead have finishtime showing time.
		if(gameInfo.mapSelected == 1) {
			instructions.text = "You made it! Your best time for each map is visible in the menu!";
		}
		// 3 stars
		if(TimePlayed < gameInfo.scoreTable[gameInfo.mapSelected-1, 0]) {
			star1.texture = Resources.Load<Texture>("Textures/star");	
			star2.texture = Resources.Load<Texture>("Textures/star");
			star3.texture = Resources.Load<Texture>("Textures/star");
		}
		// 2 stars
		else if(TimePlayed < gameInfo.scoreTable[gameInfo.mapSelected-1, 1]) {
			star1.texture = Resources.Load<Texture>("Textures/star");	
			star2.texture = Resources.Load<Texture>("Textures/star");
		}
		// 1 star
		else if(TimePlayed < gameInfo.scoreTable[gameInfo.mapSelected-1, 2]) {
			star1.texture = Resources.Load<Texture>("Textures/star");
		}
		minutes = Mathf.Floor(TimePlayed / 60);
		seconds = TimePlayed - minutes * 60;
		finishText.text = "Finished in " + minutes.ToString("00") + ":" + seconds.ToString("0#.00");

		/* Show time for each number of stars */
		float star1_minutes = Mathf.Floor(gameInfo.scoreTable[gameInfo.mapSelected-1,2]/60);
		float star2_minutes = Mathf.Floor(gameInfo.scoreTable[gameInfo.mapSelected-1,1]/60);
		float star3_minutes = Mathf.Floor(gameInfo.scoreTable[gameInfo.mapSelected-1,0]/60);
		float star1_seconds = gameInfo.scoreTable[gameInfo.mapSelected-1,2] - star1_minutes * 60;
		float star2_seconds = gameInfo.scoreTable[gameInfo.mapSelected-1,1] - star2_minutes * 60;
		float star3_seconds = gameInfo.scoreTable[gameInfo.mapSelected-1,0] - star3_minutes * 60;

		star1_time.text = star1_minutes.ToString("00") + ":" + star1_seconds.ToString("00");
		star2_time.text = star2_minutes.ToString("00") + ":" + star2_seconds.ToString("00");
		star3_time.text = star3_minutes.ToString("00") + ":" + star3_seconds.ToString("00");
	}

	/* Plays countdown sound before starting the game and shows seconds left until start */
	IEnumerator Countdown(int seconds) {
		int count = seconds;
		while (count > 0) {
			PlayBeepSound();
			countdown.GetComponentInChildren<Text>().text = count.ToString();
			yield return new WaitForSeconds(1);
			count --;
		}
		PlayStartSound();
		countdown.GetComponentInChildren<Text>().text = "GO";
		gameStarted = true;
		count = 1;
		while (count > 0) {
			yield return new WaitForSeconds(1);
			count --;
		}
		countdown.SetActive(false);
	}

}