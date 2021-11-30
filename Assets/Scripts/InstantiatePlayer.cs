using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstantiatePlayer : MonoBehaviour
{
    public GameInfo gameInfo;
    public GameObject[] startPos;
    public PlayerInput pi;
    public new Camera camera;
    public List<GameObject> playercameras;

    void Start() {
        gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();
        gameInfo.players.Clear();
        playercameras.Clear();
        startPos = GameObject.FindGameObjectsWithTag("StartPosition");
        initPlayers();
        initControls();
        initCameras();
        initPlayerUIs();
    }

    public void initPlayerUIs() {
        for (int i = 0; i < gameInfo.playerCount; i++) {
            GameObject playerUI = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerUI"));
            playerUI.GetComponentInChildren<PlayerUIScript>().player = gameInfo.players[i];
            playerUI.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            playerUI.GetComponent<Canvas>().worldCamera = playercameras[i].GetComponent<Camera>();
            gameInfo.players[i].GetComponent<PlayerController>().playerUI = playerUI;
        }
    }

    public void initPlayers() {
        for (int i = 0; i < gameInfo.playerCount; i++) {
            GameObject Player = Instantiate(gameInfo.chosenCar[i], startPos[i].transform.position, startPos[i].transform.rotation);
            Player.GetComponent<PlayerController>().playerId = i;
            if(Gamepad.all.Count > 1) {
                Player.GetComponent<PlayerController>().gp = Gamepad.all[i];
            }
            else {
                Player.GetComponent<PlayerController>().gp = Gamepad.current;
            }
            gameInfo.players.Add(Player);
        }
    }

    public void initControls() {
        for (int i = 0; i < gameInfo.playerCount; i++) {
            pi = gameInfo.players[i].GetComponent<PlayerInput>();
            switch (gameInfo.playerControls[i]) {
                case 0: // WASD
                    pi.SwitchCurrentControlScheme("WASD");
                    break;
                case 1: // ARROWS
                    pi.SwitchCurrentControlScheme("Arrows");
                    break;
                case 2: // GAMEPAD
                    pi.SwitchCurrentControlScheme("Gamepad");
                    break;
                default:
                    pi.SwitchCurrentControlScheme("WASD");
                    break;
            }
        }
    }

    public void initCameras() {
        for (int i = 0; i < gameInfo.playerCount; i++) {
            GameObject PlayerCamera = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerCamera"), startPos[i].transform.position, startPos[i].transform.rotation);
            playercameras.Add(PlayerCamera);
            RealCamera rc = PlayerCamera.GetComponent<RealCamera>();
            rc.target = gameInfo.players[i].GetComponent<Transform>();
        }
        switch (gameInfo.playerCount) {
            case 1:
                playercameras[0].GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                break;
            case 2:
                playercameras[0].GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                playercameras[1].GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                break;
            default:
                playercameras[0].GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                break;
        }
    }
}
