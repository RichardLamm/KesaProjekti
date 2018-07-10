using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerState : MonoBehaviour {

    private enum playerState
    {
        Idle,
        Moving,
        Map,
        Inventory,
        Crafting,
        Harvesting
    };

    public int speed = 10;
    private playerState state = playerState.Idle;
    private float xAxis;
    private float yAxis;
    private Vector3 cameraOffset;
    public Camera mainCamera;
    public int maxZoom;
    public int minZoom;
    public float speedModifier = 1;
    private float originalSpeedModifier;
    public float rockMovement;

    public CanvasGroup inventory;
    public CanvasGroup crafting;
    public Tilemap map;
    public Tilemap nodeMap;
    public InventoryManagement inventoryScript;

    private List<string> tools = new List<string> { "axe", "bucket", "pick", "scythe" };
    private Dictionary<string, int> items = new Dictionary<string, int>() { { "wood", 100 }, { "gold", 10 }, { "minerals", 30 } };
    
    // Use this for initialization
    void Start () {
        //Setting the camera position
        Vector3 playerPosition = GetPosition();
        mainCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y, -10);
        mainCamera.orthographicSize = minZoom;
        cameraOffset = mainCamera.transform.position - transform.position;
        originalSpeedModifier = speedModifier;
        
        //Items are added at the start of the game. Remove/upgrade these if for some reason they are called
        //before awakening the Inventory script
        inventoryScript.getItems(items);
        inventoryScript.getTools(tools);
    }


	
	// Update is called once per frame
	void Update () {
        PlayerStateMachine();       
    }

    Vector3 GetPosition()
    {
        return transform.position;
    }
    public void PlayerStateMachine()
    {
        switch (state)
        {
            case playerState.Idle:
                if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
                {
                    state = playerState.Moving;
                }
                else if (Input.GetButtonDown("Map"))
                {
                    state = playerState.Map;
                    mainCamera.orthographicSize = maxZoom;
                }

                else if (Input.GetButtonDown("Inventory"))
                {
                    inventory.alpha = 1f;
                    inventory.blocksRaycasts = true;
                    inventoryScript.getItems(items);
                    inventoryScript.getTools(tools);
                    state = playerState.Inventory;
                }

                else if (Input.GetButtonDown("Crafting"))
                {
                    crafting.alpha = 1f;
                    crafting.blocksRaycasts = true;
                    state = playerState.Crafting;
                }
                break;

            case playerState.Moving: 
                xAxis = Input.GetAxisRaw("Horizontal");
                yAxis = Input.GetAxisRaw("Vertical");

                Vector3Int gridPosition = map.WorldToCell(transform.position);
                if(map.GetSprite(gridPosition).name == "rock" || map.GetSprite(gridPosition).name == "snowyRock")
                {
                    speedModifier = rockMovement;
                }
                else
                {
                    speedModifier = originalSpeedModifier;
                }

                Vector3 movementVector = new Vector3(xAxis, yAxis, 0) * Time.deltaTime * speed * speedModifier;
                Vector3Int whereToMove = nodeMap.WorldToCell(transform.position + movementVector);
                if (nodeMap.GetTile(whereToMove) == null && map.GetTile(whereToMove) !=null)
                {
                    transform.position += movementVector;
                    mainCamera.transform.position = transform.position + cameraOffset;
                }
                
                if ( xAxis == 0 && yAxis == 0 && Input.GetButtonDown("Horizontal") == false && Input.GetButtonDown("Vertical") == false)
                {
                    state = playerState.Idle;
                }               
                break;

            case playerState.Map:
                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Vertical");
                mainCamera.transform.position += new Vector3(xAxis, yAxis, 0) * Time.deltaTime * 2 * speed;

                if (Input.GetButtonDown("Map"))
                {
                    state = playerState.Idle;
                    mainCamera.transform.position = transform.position + cameraOffset;
                    mainCamera.orthographicSize = minZoom;
                }
                break;

            case playerState.Inventory:
                inventoryScript.moveHighlight();
                if (Input.GetButtonDown("Inventory"))
                {
                    inventory.alpha = 0f;
                    inventory.blocksRaycasts = false;
                    state = playerState.Idle;
                }
                break;

            case playerState.Crafting:
                if (Input.GetButtonDown("Crafting"))
                {
                    crafting.alpha = 0f;
                    crafting.blocksRaycasts = false;
                    state = playerState.Idle;
                }
                break;
                //Resurssien keräily state
                //Paina nappia
                //Etsi lähin node/mitä jos ei ole nodea lähellä
                //kerää resursseja
                //Nosta resurssit (resurssit hyvä päivittää inventoryyn tässä vaiheessa)
                //Jos ei tilaa resurssit jäävät maahan
                //Skillien kehittyminen?
                //palaa automaattisesti Idleen kun keräys on tapahtunut
        }
    }
}
