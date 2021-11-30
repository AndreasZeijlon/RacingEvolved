using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GameInfo : MonoBehaviour {
    public List<GameObject> chosenCar;
    public int mapSelected, playerCount;
    public bool enableInstructions, enableMusic, enableEffects;
    public float[] bestTimes = new float[] {Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity};   // index is map, value is best time for that map
    public readonly float[,] scoreTable = new float[,] {{40, 60, 90}, {35, 40, 60}, {70, 75, 90} , {80, 90, 100}, {85, 90, 100}}; // Time for differents scores on maps
    public int[] playerControls, carSelected;
    public List<GameObject> players;

    private static GameInfo gameInfoInstance;


    void Start() {
        SceneManager.sceneLoaded += this.OnLoadCallback;
    }


    void Awake() {
        DontDestroyOnLoad (this);
        if (gameInfoInstance == null) {
            gameInfoInstance = this;
        } else {
            Destroy(gameObject);
        }
        carSelected = new int [] {0, 0, 0, 0};
        playerControls = new int [] {0, 1, 2, 2};
        mapSelected = 1;
        enableInstructions = false;
        enableMusic = true;
        enableEffects = true;
        LoadGame();
    }


    void OnLoadCallback (Scene scene, LoadSceneMode mode) {
        if(scene.name == "menu") {
            Start();
        }
    } 


    private Save CreateSaveGameObject() {
        Save save = new Save();
        save.bestScores = bestTimes;
        save.enableMusic = enableMusic;
        save.enableInstructions = enableInstructions;
        save.enableEffects = enableEffects;
        save.playerControls = playerControls;
        
        return save;
    }


    public void SaveGame() {
        Save save = CreateSaveGameObject();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }


    public void LoadGame() { 
        if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();
            bestTimes = save.bestScores;
            enableMusic = save.enableMusic;
            enableInstructions = save.enableInstructions;
            enableEffects = save.enableEffects;
            playerControls = save.playerControls;
            
        }
    }
}
