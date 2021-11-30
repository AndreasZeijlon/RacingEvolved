using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    public GameObject player, parent, boost;
    public Text speed, id;
    public Rect viewPort;
    public int offset = 50;
    public GameInfo gameInfo;

    // Start is called before the first frame update
    void Start()
    { 
        gameInfo = GameObject.Find("GameInfo").GetComponent<GameInfo>();
        gameObject.GetComponentInParent<Canvas>().planeDistance = 1;
        viewPort = gameObject.GetComponentInParent<Canvas>().pixelRect;
        parent = gameObject.transform.parent.gameObject;
        boost = parent.transform.GetChild(0).gameObject;
        
        Text[] UIElements = parent.GetComponentsInChildren<Text>();

        speed = UIElements[0];
        id = UIElements[1];
        speed.fontSize = 28;
        id.fontSize = 28;


        boost.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -viewPort.height/2 + offset);
        speed.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -viewPort.height/2 + offset*2);
        id.GetComponent<RectTransform>().anchoredPosition = new Vector2(-viewPort.width/2 + offset*2, viewPort.height/2 - offset/2);

        if(gameInfo.playerCount == 1 ) {
            id.gameObject.SetActive(false);
        } else {
            id.text = "Player " + (player.GetComponentInChildren<PlayerController>().playerId + 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        boost.GetComponent<Slider>().value = player.GetComponentInChildren<PlayerController>().boost;
        speed.text = (player.GetComponentInChildren<PlayerController>().rb.velocity.magnitude * 3.6f).ToString("N0") + " km/h";
    }
}
