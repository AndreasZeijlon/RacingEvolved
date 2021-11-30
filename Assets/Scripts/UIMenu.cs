using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class UIMenu : MonoBehaviour {
    /* Used to hide/show relevant buttons/info at each state */
    private enum MenuStates {
        main, settings, singleplayer, splitscreen, controls
    };
    private GameObject currentState, main_menu, settings_menu, singleplayer_menu, splitscreen_menu, controls_menu, mainmenu_button_object, startgame_button_object, stars_per_map, lockCar, lockCar1, lockCar2, lockMap, lockMapSplit, stars_collected, stars_to_unlock_car, stars_to_unlock_car1, stars_to_unlock_car2, stars_to_unlock_map1, stars_to_unlock_map2, car_info, car_info_player1, car_info_player2;
    private RawImage carImg, carImg1, carImg2, mapImg, mapImg2, star1, star2, star3;
    public List<GameObject> carList;
    public List<Texture2D> mapList;     // Preview images for each map
    private readonly string[] mapNames = {"Sand", "Canyon", "Village", "City", "Gravel Pit"};
    private List<List<float>> carStats; // Used to show stats of cars in menu
    private Button start_button, singleplayer_button, splitscreen_button, settings_button, quit_button, mainmenu_button, settings_back_button, controls_button, newgame_back_button, leftcar, rightcar,leftcar1, rightcar1, leftcar2, rightcar2, startgame_button, leftmap, rightmap, leftmap2, rightmap2;
    private Toggle instructions_toggle, music_toggle, effects_toggle;
	private Dropdown player1_controls, player2_controls;
    private Text best_time, player1_controls_info, player2_controls_info, map_name, map_name_split;
    private int numOfCars, numOfMaps, numOfStars;
    private GameInfo gameInfo;
    private int[] unlock_cars, unlock_maps;
    private bool mapUnlocked, car1Unlocked, car2Unlocked;
    public List<AudioSource> audioSources; // Backgroundmusic and button press sound 

    void Start() {
        carStats = new List<List<float>>();
        mapUnlocked = true;     // Startmap
        car1Unlocked = true;    // Startcar
        car2Unlocked = true;    // startcar

        numOfCars = 8;
        numOfMaps = 5;

        unlock_cars = new int[] {0, 3, 6, 8, 10, 12, 15, 15};    // Points needed for each car              {0, 0, 0, 0, 0, 0, 0, 0};
        unlock_maps = new int[] {0, 2, 5, 8, 10};                // Points needed for each map              {0, 0, 0, 0, 0}; 
        linkGameObjects();
        gameInfo.players.Clear();
        initCarList();
        initMapList();
        initAudioSources();
        initCarStats();
        changeCarInfoText(gameInfo.carSelected[0], 0);
        changeCarInfoText(gameInfo.carSelected[1], 1);
        changeMapName(gameInfo.mapSelected-1);
        CountNumberOfStars();
        initMenuStars();
        
        instructions_toggle.isOn = gameInfo.enableInstructions;
        music_toggle.isOn = gameInfo.enableMusic;
        effects_toggle.isOn = gameInfo.enableEffects;

        main_menu.SetActive(false);
        settings_menu.SetActive(false);
        singleplayer_menu.SetActive(false);
        splitscreen_menu.SetActive(false);
        controls_menu.SetActive(false);

        mainmenu_button_object.SetActive(false);
        startgame_button_object.SetActive(false);
        stars_per_map.SetActive(false);
  
        gameInfo.chosenCar.Clear();

        initControls();

        HideLockedImage(lockCar);
        HideLockedImage(lockMap);
        HideLockedImage(lockCar1);
        HideLockedImage(lockCar2);
        HideLockedImage(lockMapSplit);

        UpdateCarImage(0);
        UpdateCarImage(1);
        UpdateMapImage();
        UpdateCarButtonInteractability(0);
        UpdateCarButtonInteractability(1);
        UpdateMapButtonInteractability();
        updateStarsRequiredMap();
        updateStarsRequiredCar(0);
        updateStarsRequiredCar(1);
        placeButtonsInCorners();

        currentState = main_menu;
        switchMenu(MenuStates.main);
        PlayMusic();
    }

    public void initMapList() {
        mapList.Add(Resources.Load<Texture2D>("Textures/mapimages/startmap"));
        mapList.Add(Resources.Load<Texture2D>("Textures/mapimages/terrainmap"));
        mapList.Add(Resources.Load<Texture2D>("Textures/mapimages/naturemap"));
        mapList.Add(Resources.Load<Texture2D>("Textures/mapimages/newcitymap"));
        mapList.Add(Resources.Load<Texture2D>("Textures/mapimages/constructionmap"));
    }

    public void initCarList() {
        carList.Add(Resources.Load<GameObject>("Prefabs/startcar"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Car_2"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Car_3"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Car_4"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Taxi"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Police_car"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Bus_1"));
        carList.Add(Resources.Load<GameObject>("Prefabs/Bus_2"));
    }

    public void initAudioSources() {
        audioSources.Add(GetComponent<AudioSource>());
        audioSources.Add(GameObject.Find("PlayButtonSound").GetComponent<AudioSource>());
    }

    public void initControls() {
        player1_controls.value = gameInfo.playerControls[0];
        player2_controls.value = gameInfo.playerControls[1];
        player1_controls_info.text = changeControlsInfo(gameInfo.playerControls[0]);
        player2_controls_info.text = changeControlsInfo(gameInfo.playerControls[1]);
    }

    public void initMenuStars() {
        stars_collected.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width/2-100, Screen.height/2-60);
        stars_collected.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_collected.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

        stars_to_unlock_car.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_to_unlock_car.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

        stars_to_unlock_car1.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_to_unlock_car1.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

        stars_to_unlock_car2.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_to_unlock_car2.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

        stars_to_unlock_map1.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_to_unlock_map1.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

        stars_to_unlock_map2.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("Textures/star");
        stars_to_unlock_map2.GetComponentInChildren<Text>().text = numOfStars + "/" + numOfMaps*3;

    }

    public void placeButtonsInCorners() {
        mainmenu_button_object.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width/2+180, -Screen.height/2+60);
        startgame_button_object.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width/2-180, -Screen.height/2+60);
    }

    public void CountNumberOfStars() {
        for (int i = 0; i < gameInfo.bestTimes.Length; i++)
        {
            if(gameInfo.bestTimes[i] < gameInfo.scoreTable[i, 0]) {
                numOfStars += 3;
            } else if(gameInfo.bestTimes[i] < gameInfo.scoreTable[i, 1]) {
                numOfStars += 2;

            } else if(gameInfo.bestTimes[i] < gameInfo.scoreTable[i, 2]) {
                numOfStars += 1;
            }
        }
    }

    public void linkGameObjects() {
        gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();

        main_menu = GameObject.Find("main_menu");
        settings_menu = GameObject.Find("settings_menu");
        singleplayer_menu = GameObject.Find("singleplayer_menu");
        splitscreen_menu = GameObject.Find("splitscreen_menu");
        controls_menu = GameObject.Find("controls_menu");
        mainmenu_button_object = GameObject.Find("mainmenu_button_object");     // Used to hide/show mainmenu button
        startgame_button_object = GameObject.Find("startgame_button_object");   // Used to hide/show startgame button
        stars_per_map = GameObject.Find("stars_per_map");
        lockCar = GameObject.Find("lockCarImgRaw");
        lockCar1 = GameObject.Find("lockCar1ImgRaw");
        lockCar2 = GameObject.Find("lockCar2ImgRaw");
        lockMapSplit = GameObject.Find("lockMapImgRawSplit");
        lockMap = GameObject.Find("lockMapImgRaw");
        stars_collected = GameObject.Find("Stars_collected");
        stars_to_unlock_car = GameObject.Find("stars_to_unlock_car");
        stars_to_unlock_car1 = GameObject.Find("stars_to_unlock_car1");
        stars_to_unlock_car2 = GameObject.Find("stars_to_unlock_car2");
        stars_to_unlock_map1 = GameObject.Find("stars_to_unlock_map1");
        stars_to_unlock_map2 = GameObject.Find("stars_to_unlock_map2");

        carImg = GameObject.Find("carImgRaw").GetComponent<RawImage>();
        carImg1 = GameObject.Find("carImgRaw1").GetComponent<RawImage>();
        carImg2 = GameObject.Find("carImgRaw2").GetComponent<RawImage>();
        mapImg = GameObject.Find("mapImgRaw").GetComponent<RawImage>();
        mapImg2 = GameObject.Find("mapImgRaw2").GetComponent<RawImage>();
        star1 = GameObject.Find("Star1").GetComponent<RawImage>();
        star2 = GameObject.Find("Star2").GetComponent<RawImage>();
        star3 = GameObject.Find("Star3").GetComponent<RawImage>();

        singleplayer_button = GameObject.Find("singleplayer_button").GetComponent<Button>();
        splitscreen_button = GameObject.Find("splitscreen_button").GetComponent<Button>();
        settings_button = GameObject.Find("settings_button").GetComponent<Button>();
        quit_button = GameObject.Find("quit_button").GetComponent<Button>();
        mainmenu_button = GameObject.Find("mainmenu_button").GetComponent<Button>();
        startgame_button = GameObject.Find("startgame_button").GetComponent<Button>();
        controls_button = GameObject.Find("controls_button").GetComponent<Button>();
        leftcar = GameObject.Find("leftcar").GetComponent<Button>();
        rightcar = GameObject.Find("rightcar").GetComponent<Button>();
        leftcar1 = GameObject.Find("leftcar1").GetComponent<Button>();
        rightcar1 = GameObject.Find("rightcar1").GetComponent<Button>();
        leftcar2 = GameObject.Find("leftcar2").GetComponent<Button>();
        rightcar2 = GameObject.Find("rightcar2").GetComponent<Button>();
        leftmap = GameObject.Find("leftmap").GetComponent<Button>();
        rightmap = GameObject.Find("rightmap").GetComponent<Button>();
        leftmap2 = GameObject.Find("leftmap2").GetComponent<Button>();
        rightmap2 = GameObject.Find("rightmap2").GetComponent<Button>();

        instructions_toggle = GameObject.Find("instructions_toggle").GetComponent<Toggle>();
        music_toggle = GameObject.Find("music_toggle").GetComponent<Toggle>();
        effects_toggle = GameObject.Find("effects_toggle").GetComponent<Toggle>();

        player1_controls = GameObject.Find("player1_dropdown").GetComponent<Dropdown>();
        player2_controls = GameObject.Find("player2_dropdown").GetComponent<Dropdown>();
        player1_controls_info = GameObject.Find("Player1_info").GetComponent<Text>();
        player2_controls_info = GameObject.Find("Player2_info").GetComponent<Text>();
        map_name = GameObject.Find("map_name").GetComponent<Text>();
        map_name_split = GameObject.Find("map_name_split").GetComponent<Text>();

        car_info = GameObject.Find("car_info");
        car_info_player1 = GameObject.Find("car_info_player1");
        car_info_player2 = GameObject.Find("car_info_player2");

        singleplayer_button.onClick.AddListener(OnSingleplayer);
        splitscreen_button.onClick.AddListener(OnSplitscreen);
        settings_button.onClick.AddListener(OnSettings);
        quit_button.onClick.AddListener(OnQuit);
        startgame_button.onClick.AddListener(OnStartGame);
        mainmenu_button.onClick.AddListener(OnMainMenu);
        controls_button.onClick.AddListener(OnControls);
        instructions_toggle.onValueChanged.AddListener((delegate {OnInstructionsToggle(instructions_toggle);}));
        music_toggle.onValueChanged.AddListener((delegate {OnMusicToggle(music_toggle);}));
        effects_toggle.onValueChanged.AddListener((delegate {OnEffectsToggle(effects_toggle);}));
        player1_controls.onValueChanged.AddListener(delegate {OnControlsChanged(player1_controls, 0);});
        player2_controls.onValueChanged.AddListener(delegate {OnControlsChanged(player2_controls, 1);});

        leftcar.onClick.AddListener(delegate {OnCarLeft(0);});      // Singleplayer
        rightcar.onClick.AddListener(delegate {OnCarRight(0);});    // Singleplayer
        leftcar1.onClick.AddListener(delegate {OnCarLeft(0);});     // Player 1
        rightcar1.onClick.AddListener(delegate {OnCarRight(0);});   // Player 1
        leftcar2.onClick.AddListener(delegate {OnCarLeft(1);});     // Player 2
        rightcar2.onClick.AddListener(delegate {OnCarRight(1);});   // Player 2
        leftmap.onClick.AddListener(OnMapLeft);
        rightmap.onClick.AddListener(OnMapRight);
        leftmap2.onClick.AddListener(OnMapLeft);
        rightmap2.onClick.AddListener(OnMapRight);
    }


    public void OnControlsChanged(Dropdown change, int player) {
        PlayButtonSound(); // Will play at beginning each time..
        gameInfo.playerControls[player] = change.value;
        if(player==0) {
            player1_controls_info.text = changeControlsInfo(change.value);
        } else if (player==1){
            player2_controls_info.text = changeControlsInfo(change.value);
        }
    }


    private string changeControlsInfo(int playerControls) {
        string info, throttleKey, turnKeys, breakKey, boostKey, reverseKey, resetKey, honkKey, pauseKey;
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
        info = "Throttle: " + throttleKey + "\nReverse: " + reverseKey + "\nTurn: " + turnKeys + "\nBreak: " + breakKey + "\nBoost: " + boostKey + "\nReset: " + resetKey + "\nPause: " + pauseKey + "\nHonk: " + honkKey;
        return info;
	}

    /* Menubuttons changing "scenes" ----------------------------------------------------- */
    public void OnStartGame() {
        Debug.Log("start game");
        /* Can not start game with any locked cars/maps */
        if( !mapUnlocked || !car1Unlocked || !car2Unlocked ) {
            // Do nothing
            Debug.Log("start game");

        } 
        else {
            if(currentState == singleplayer_menu) {
                gameInfo.playerCount = 1;
            }
            else {
                gameInfo.playerCount = 2;
            }
            for (int i = 0; i < gameInfo.playerCount; i++) {
                gameInfo.chosenCar.Add(carList[gameInfo.carSelected[i]]);
            }           
            /* Map is same for everyone */
            SceneManager.LoadScene(gameInfo.mapSelected); 
        } 
    }

    public void OnSingleplayer() {
        PlayButtonSound();
        switchMenu(MenuStates.singleplayer);
    }

    public void OnSettings() {
        PlayButtonSound();
        switchMenu(MenuStates.settings);
    }

    public void OnMainMenu() {
        PlayButtonSound();
        switchMenu(MenuStates.main);
    }

    public void OnQuit() {
        gameInfo.SaveGame();
        Application.Quit();
    }

    public void OnControls() {
        PlayButtonSound();
       switchMenu(MenuStates.controls);
    }

     public void OnInstructionsToggle(Toggle instructions_toggle) {
         if(instructions_toggle.isOn) {
              gameInfo.enableInstructions = true;
         }
         else {
              gameInfo.enableInstructions = false;
         }
    }

        public void OnMusicToggle(Toggle music_toggle) {
            if(music_toggle.isOn) {
                gameInfo.enableMusic = true;
                PlayPauseMusic();
            }
            else {
                gameInfo.enableMusic = false;
                PlayPauseMusic();
            }
    }

         public void OnEffectsToggle(Toggle effects_toggle) {
            if(effects_toggle.isOn) {
                gameInfo.enableEffects = true;
            }
            else {
                gameInfo.enableEffects = false;
            }
    }

    public void OnSplitscreen() {
        PlayButtonSound();
        switchMenu(MenuStates.splitscreen);
    }

    public void UpdateCarButtonInteractability(int player) {
        if(player == 0) {
            if(gameInfo.carSelected[player] == 0) {
                leftcar.interactable = false;
                leftcar1.interactable = false;
                rightcar.interactable = true;
                rightcar1.interactable = true;
            } else if (gameInfo.carSelected[player] == numOfCars-1) {
                leftcar.interactable = true;
                leftcar1.interactable = true;
                rightcar.interactable = false;
                rightcar1.interactable = false;
            } else {
                leftcar.interactable = true;
                leftcar1.interactable = true;
                rightcar.interactable = true;
                rightcar1.interactable = true;
            }
        } else if (player == 1) {
            if(gameInfo.carSelected[player] == 0) {
                leftcar2.interactable = false;
                rightcar2.interactable = true;
            } else if (gameInfo.carSelected[player] == numOfCars-1) {
                leftcar2.interactable = true;
                rightcar2.interactable = false;
            } else {
                leftcar2.interactable = true;
                rightcar2.interactable = true;
            }
        }
    }

    public void UpdateMapButtonInteractability() {
        if(gameInfo.mapSelected == 1) {
            leftmap.interactable = false;
            leftmap2.interactable = false;
            rightmap.interactable = true;
            rightmap2.interactable = true;
        } 
        else if (gameInfo.mapSelected == numOfMaps) {
            leftmap.interactable = true;
            leftmap2.interactable = true;
            rightmap.interactable = false;
            rightmap2.interactable = false;            
        } 
        else {
            leftmap.interactable = true;
            leftmap2.interactable = true;
            rightmap.interactable = true;
            rightmap2.interactable = true;   
        }
    }

    public void updateStarsRequiredCar(int player) {
        if (player==0) {
            if(car1Unlocked) {
                stars_to_unlock_car.SetActive(false);
                stars_to_unlock_car1.SetActive(false);
            } else {
                stars_to_unlock_car.SetActive(true);
                stars_to_unlock_car.GetComponentInChildren<Text>().text = numOfStars + "/" + unlock_cars[gameInfo.carSelected[player]];

                stars_to_unlock_car1.SetActive(true);
                stars_to_unlock_car1.GetComponentInChildren<Text>().text = numOfStars + "/" + unlock_cars[gameInfo.carSelected[player]];
            }  
        } 
        else if (player == 1) {
            if(car2Unlocked) {
                stars_to_unlock_car2.SetActive(false);
            } else {
                stars_to_unlock_car2.SetActive(true);
                stars_to_unlock_car2.GetComponentInChildren<Text>().text = numOfStars + "/" + unlock_cars[gameInfo.carSelected[player]];
            }  
        }
    }

    public void updateStarsRequiredMap() {
        if(mapUnlocked) {
            stars_to_unlock_map1.SetActive(false);
            stars_to_unlock_map2.SetActive(false);
        } else {
            stars_to_unlock_map2.SetActive(true);
            stars_to_unlock_map1.SetActive(true);
            stars_to_unlock_map1.GetComponentInChildren<Text>().text = numOfStars + "/" + unlock_maps[gameInfo.mapSelected-1];

            stars_to_unlock_map2.GetComponentInChildren<Text>().text = numOfStars + "/" + unlock_maps[gameInfo.mapSelected-1];
        }
    }

    public void OnCarLeft(int player) {
        if(gameInfo.carSelected[player] > 0) {
            PlayButtonSound();
            gameInfo.carSelected[player] --;
            if( numOfStars < unlock_cars[gameInfo.carSelected[player]]) {
                if (player == 0) {
                    car1Unlocked = false;
                } else {
                    car2Unlocked = false;
                }
                ToggleStartButtonInteractable(false);
            } else {
                if (player == 0) {
                    car1Unlocked = true;
                } else {
                    car2Unlocked = true;
                }
                ToggleStartButtonInteractable(true);
            }
            updateStarsRequiredCar(player);
            UpdateCarButtonInteractability(player);
            UpdateCarImage(player);
            changeCarInfoText(gameInfo.carSelected[player], player);
        }
    }

    public void OnCarRight(int player) {
        if(gameInfo.carSelected[player] < numOfCars-1) {
            PlayButtonSound();
            gameInfo.carSelected[player] ++;
            if( numOfStars < unlock_cars[gameInfo.carSelected[player]]) {
                if (player == 0) {
                    car1Unlocked = false;
                } else {
                    car2Unlocked = false;
                }
                ToggleStartButtonInteractable(false);
            } else {
                if (player == 0) {
                    car1Unlocked = true;
                } else {
                    car2Unlocked = true;
                }
                ToggleStartButtonInteractable(true);
            }
            updateStarsRequiredCar(player);
            UpdateCarButtonInteractability(player);
            UpdateCarImage(player);
            changeCarInfoText(gameInfo.carSelected[player], player);
        }
    }

    public void OnMapLeft() {
        if(gameInfo.mapSelected > 1) {
            PlayButtonSound();
            gameInfo.mapSelected --;
            if( numOfStars < unlock_maps[gameInfo.mapSelected-1]) {
                mapUnlocked = false;
                ToggleStartButtonInteractable(false);
            } else {
                mapUnlocked = true;
                ToggleStartButtonInteractable(true);
            }
            updateStarsRequiredMap();
            UpdateMapButtonInteractability();
            UpdateMapImage();
            changeMapName(gameInfo.mapSelected-1);
        }
    }

    public void OnMapRight() {
        if(gameInfo.mapSelected < numOfMaps) {
            PlayButtonSound();
            gameInfo.mapSelected ++;
            if( numOfStars < unlock_maps[gameInfo.mapSelected-1]) {
                ToggleStartButtonInteractable(false);
                mapUnlocked = false;
            } else {
                mapUnlocked = true;
                ToggleStartButtonInteractable(true);
            }
            updateStarsRequiredMap();

            UpdateMapButtonInteractability();
            UpdateMapImage();
            changeMapName(gameInfo.mapSelected-1);

        }
    }

    public void HideLockedImage(GameObject obj) {
        obj.SetActive(false);
    }

    public void ShowLockedImage(GameObject obj) {
        RawImage rawImg = obj.GetComponent<RawImage>();
        rawImg.texture = Resources.Load<Texture2D>("Textures/lock5");
        obj.SetActive(true);
    }

    public void UpdateCarImage(int player) {
        if (currentState == singleplayer_menu) {
            carImg.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[player]);
            carImg1.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[player]);
            if(!car1Unlocked) {
                ShowLockedImage(lockCar);
                ShowLockedImage(lockCar1);
            } else {
                HideLockedImage(lockCar1);
                HideLockedImage(lockCar);
            }
        } 
        else if(currentState == splitscreen_menu) {
            if (player == 0) {
                carImg.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[player]);
                carImg1.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[player]);
                if(!car1Unlocked) {
                    ShowLockedImage(lockCar);
                    ShowLockedImage(lockCar1);
                } else {
                    HideLockedImage(lockCar1);
                    HideLockedImage(lockCar);
                }
            }
            else if(player == 1) {
                carImg2.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[player]);
                if(!car2Unlocked) {
                    ShowLockedImage(lockCar2);
                } else {
                    HideLockedImage(lockCar2);
                }
            }
        }
        else {
            carImg.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[0]);
            carImg1.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[0]);
            carImg2.texture = Resources.Load<Texture2D>("Textures/car_"+gameInfo.carSelected[1]);
        }
    }

    public void UpdateMapImage() {
        mapImg.texture = mapList[gameInfo.mapSelected-1];    // Singleplayer
        mapImg2.texture = mapList[gameInfo.mapSelected-1];   // Splitscreen

        if(!mapUnlocked) {
            ShowLockedImage(lockMap);
            ShowLockedImage(lockMapSplit);
        } 
        else {
            HideLockedImage(lockMap);
            HideLockedImage(lockMapSplit);
        } 

        float bestTime = gameInfo.bestTimes[gameInfo.mapSelected-1];
        if(bestTime == Mathf.Infinity) {
            stars_per_map.GetComponentInChildren<Text>().text = "--:--.--";
        } 
        else {
            float bestTime_minutes = Mathf.Floor(bestTime / 60);
            float bestTime_seconds = bestTime - bestTime_minutes * 60;
            stars_per_map.GetComponentInChildren<Text>().text = bestTime_minutes.ToString("00") + ":" + bestTime_seconds.ToString("0#.00");
        }
        // 3 stars
        if(bestTime < gameInfo.scoreTable[gameInfo.mapSelected-1, 0]) {
            star1.texture = Resources.Load<Texture>("Textures/star");	
            star2.texture = Resources.Load<Texture>("Textures/star");
            star3.texture = Resources.Load<Texture>("Textures/star");
        }
        // 2 stars
        else if(bestTime < gameInfo.scoreTable[gameInfo.mapSelected-1, 1]) {
            star1.texture = Resources.Load<Texture>("Textures/star");	
            star2.texture = Resources.Load<Texture>("Textures/star");
            star3.texture = Resources.Load<Texture>("Textures/grey_star");
        }
        // 1 star
        else if(bestTime < gameInfo.scoreTable[gameInfo.mapSelected-1, 2]) {
            star1.texture = Resources.Load<Texture>("Textures/star");	
            star2.texture = Resources.Load<Texture>("Textures/grey_star");
            star3.texture = Resources.Load<Texture>("Textures/grey_star");
        } 
        else {
            star1.texture = Resources.Load<Texture>("Textures/grey_star");	
            star2.texture = Resources.Load<Texture>("Textures/grey_star");
            star3.texture = Resources.Load<Texture>("Textures/grey_star");                
        }
    }
    /* End setup of cars and map ----------------------------------------------------*/


    /* Handles which menu is shown and what buttons are visible. Should always be able to get back to main menu. Should not be able to start game without chosing map/car(s). */
    private void switchMenu(MenuStates menu) {
        GameObject newState;
        switch (menu) {
            case MenuStates.main:
                newState = main_menu;
                mainmenu_button_object.SetActive(false);
                startgame_button_object.SetActive(false);
                stars_per_map.SetActive(false);
                break;
            case MenuStates.settings:
                newState = settings_menu;
                mainmenu_button_object.SetActive(true);
                startgame_button_object.SetActive(false);
                break;
            case MenuStates.singleplayer:
                newState = singleplayer_menu;
                mainmenu_button_object.SetActive(true);
                startgame_button_object.SetActive(true);
                stars_per_map.SetActive(true);
                break;
            case MenuStates.splitscreen:
                newState = splitscreen_menu;
                mainmenu_button_object.SetActive(true);
                startgame_button_object.SetActive(true);
                stars_per_map.SetActive(true);
                break;
	        case MenuStates.controls:
                newState = controls_menu;
                mainmenu_button_object.SetActive(true);
                startgame_button_object.SetActive(false);
                break;
            default:
                newState = main_menu;
                mainmenu_button_object.SetActive(false);
                startgame_button_object.SetActive(false);
                break;
        }
        currentState.SetActive(false);
        currentState = newState;
        currentState.SetActive(true);
    }

    public void PlayPauseMusic() {
        if(audioSources[0].isPlaying) {
            audioSources[0].Pause(); 
        }
        else if (gameInfo.enableMusic) {
            audioSources[0].Play(); 
        }
    }

    public void PlayMusic() {
        if(gameInfo.enableMusic) {
            audioSources[0].Play(); 
        }
    }

    public void PlayButtonSound() {
        if(gameInfo.enableEffects) {
            audioSources[1].Play(); 
        }
    }

    private void ToggleStartButtonInteractable(bool isEnabled) {
        startgame_button.interactable = isEnabled;
    }                

    public void initCarStats() {
        float carInfoSpeed, carInfoAcceleration, carInfoBoost;
        CarStats cs;
        for (int i = 0; i < carList.Count; i++) {
            cs = carList[i].GetComponent<CarStats>();
            carInfoSpeed = cs.maxSpeed/30f;
            carInfoAcceleration = cs.maxMotorTorque/cs.mass;
            carInfoBoost = (cs.boostDuration/cs.boostRegen)*3F;
            carStats.Add(new List<float> {carInfoSpeed,carInfoAcceleration ,carInfoBoost });
        }
    }

    public void changeCarInfoText(int carSelected, int player) {
        if (player==0) {
            Slider[] info_elements = car_info.GetComponentsInChildren<Slider>();
            Slider[] info_elements1 = car_info_player1.GetComponentsInChildren<Slider>();
            info_elements[0].value = carStats[carSelected][0];
            info_elements[1].value = carStats[carSelected][1];
            info_elements[2].value = carStats[carSelected][2];
            info_elements1[0].value = carStats[carSelected][0];
            info_elements1[1].value = carStats[carSelected][1];
            info_elements1[2].value = carStats[carSelected][2];
        } else if (player==1) {
            Slider[] info_elements2 = car_info_player2.GetComponentsInChildren<Slider>();
            info_elements2[0].value = carStats[carSelected][0];
            info_elements2[1].value = carStats[carSelected][1];
            info_elements2[2].value = carStats[carSelected][2];
        }
    }

    public void changeMapName(int mapSelected) {
        map_name.text = mapNames[mapSelected];
        map_name_split.text = mapNames[mapSelected];

    }
}
