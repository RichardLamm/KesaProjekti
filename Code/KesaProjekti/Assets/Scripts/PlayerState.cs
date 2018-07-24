using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public BoundsInt area;
    public int speed = 10;
    private playerState state = playerState.Idle;
    private float xAxis;
    private float yAxis;
    private Vector3 cameraOffset;
    public Camera mainCamera;
    public int maxZoom;
    public int minZoom;
    public float speedModifier = 1;
    private float originalSpeedModifier = 1;
    public float rockMovement;
    public float bufferSize = 0.5f;

    public CanvasGroup inventoryUI;
    public CanvasGroup crafting;
    public Tilemap map;
    public Tilemap nodeMap;
    public InventoryManagement inventoryScript;
    public MapGeneration mapScript;

    private List<string> tools = new List<string> { "axe", "bucket", "pick", "scythe" };
    private Dictionary<string, int> items = new Dictionary<string, int>() { { "wood", 100 }, { "gold", 10 }, { "minerals", 30 } };
    
    // Use this for initialization
    void Start () {
        
    }

    public void SetSelfPosition()
    {
        //Setting the camera position
        Vector3 playerPosition = GetPosition();
        transform.position = playerPosition;
        mainCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y, -10);
        mainCamera.orthographicSize = minZoom;
        cameraOffset = mainCamera.transform.position - transform.position;

        inventoryScript.CreateInventorySlots();
        inventoryScript.CreateStartingInventory();
        inventoryScript.GetItems();
        inventoryScript.GetTools();
    }
	
	// Update is called once per frame
	void Update () {
        PlayerStateMachine();       
    }

    Vector3 GetPosition()
    {
        Dictionary<string, List<MapGeneration.SpawnPoint>> spawnDict = GameObject.Find("Grid").GetComponent<MapGeneration>().GetSpawnPoints();
        List<MapGeneration.SpawnPoint> possibleSpawns = new List<MapGeneration.SpawnPoint>();
        foreach(string type in spawnDict.Keys)
        {
            if(type == "grass" || type == "ground")
            {
                possibleSpawns.AddRange(spawnDict[type]);
            }
        }
        // Just to ensure that if no spawns are found, the game won't crash
        if(possibleSpawns.Count == 0) { return transform.position; }
        int random = 0;
        MapGeneration.SpawnPoint spawn = new MapGeneration.SpawnPoint();
        // To ensure that player won't spawn in a node
        do
        {
            random = Random.Range(0, possibleSpawns.Count - 1);
            spawn = possibleSpawns[random];
            possibleSpawns.Remove(spawn);
        } while (nodeMap.GetTile(new Vector3Int(spawn.x, spawn.y, 0)) != null);
        return new Vector3(spawn.x, spawn.y, 0);
    }

    float createBuffer(float direction)
    {
        if (direction > 0)
        {
            direction = direction + bufferSize;
        }

        else if (direction < 0)
        {
            direction = direction - bufferSize;
        }
        return direction;
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
                    inventoryUI.alpha = 1f;
                    inventoryUI.blocksRaycasts = true;
                    //inventoryScript.getItems(items);
                    //inventoryScript.getTools(tools);
                    state = playerState.Inventory;
                }

                else if (Input.GetButtonDown("Craft"))
                {
                    crafting.alpha = 1f;
                    crafting.blocksRaycasts = true;
                    state = playerState.Crafting;
                }

                else if (Input.GetButtonDown("Harvest"))
                {                   
                    state = playerState.Harvesting;
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

                float diagonalSpeed = (float)Mathf.Sqrt((yAxis * yAxis) + (xAxis * xAxis));
                if (diagonalSpeed == 0f)
                {
                    diagonalSpeed = 1;
                }

                
                Vector3 movementVector = new Vector3(xAxis, yAxis, 0) * Time.deltaTime * (speed/diagonalSpeed) * speedModifier;
                Vector3 bufferVector = movementVector;
                bufferVector.x = createBuffer(movementVector.x)/2;
                bufferVector.y = createBuffer(movementVector.y);
                Vector3Int whereToMove = nodeMap.WorldToCell(transform.position + bufferVector);

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
                inventoryScript.MoveHighlight();
                if (Input.GetButtonDown("Inventory"))
                {
                    inventoryUI.alpha = 0f;
                    inventoryUI.blocksRaycasts = false;
                    state = playerState.Idle;
                }
                break;

            case playerState.Crafting:
                if (Input.GetButtonDown("Craft"))
                {
                    crafting.alpha = 0f;
                    crafting.blocksRaycasts = false;
                    state = playerState.Idle;
                }
                break;

            case playerState.Harvesting:
                List<Tile> nodes = new List<Tile> ();
                var playerPosition = map.WorldToCell(transform.position);
                playerPosition.x = playerPosition.x - 1;
                playerPosition.y = playerPosition.y - 1;
                area.position = map.WorldToCell(playerPosition);

                var temp = nodeMap.GetTilesBlock(area);
                foreach (var tile in temp)
                {
                    if (tile != null)
                    {
                        nodes.Add((Tile)tile);                        
                    }
                }

                // If there are no nodes around, skip this
                if (nodes.Count != 0)
                {
                    TileScript.GatherPair pair = new TileScript.GatherPair();
                    TileScript nodesScript = nodes[0].gameObject.GetComponent<TileScript>();
                    Thread gatherThread = new Thread(() => { pair = nodesScript.Gather("axe"); });
                    gatherThread.Start();
                    while (gatherThread.IsAlive)
                    {
                        // TODO: interrupt with certain key event
                        if (Input.GetButtonDown("Interrupt")){ gatherThread.Abort(); }
                    }
                    gatherThread.Join();
                    int amount = (int)pair.gatherAmount;
                    string key = pair.gatherName;
                    if (items.ContainsKey(key) != false)
                    {
                        items[key] = items[key] + amount;
                    }
                    else
                    {
                        items[key] = amount;
                    }
                    inventoryScript.inventoryChanged = true;
                    //inventoryScript.valueNeedsUpdating("minerals", amount);
                }
                state = playerState.Idle;
                break;
        }
    }
}
