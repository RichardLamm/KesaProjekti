using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGeneration : MonoBehaviour {

    public Tilemap map;
    public Tilemap nodes;
    public Tile grass;
    public Tile ground;
    public Tile water;
    public Tile tree;
    public Tile rock;
    public Tile snowyRock;
    public Tile ore;
    public GameObject TilePrefab;
    public int width = 640;
    public int height = 480;
    public float scale = 5f;
    public float groundTrigger = 0.1f;
    public float grassTrigger = 0.3f;
    public int numberOfRivers = 3;
    public int offset = 0;
    //private enum direction {none, north, northeast, east, southeast, south, southwest, west, northwest};
    public int randomLength = 30;
    public int centerRadius = 10;
    private int lastDirection = -1;


    public struct SpawnPoint
    {
        public int x;
        public int y;
        public Tile onTile;
        public SpawnPoint(int x_, int y_, Tile onTile_)
        {
            x = x_;
            y = y_;
            onTile = onTile_;
        }
    }

    private struct TileRules
    {
        private Tile self;
        private List<Tile> limitations;
        public TileRules(Tile self_, List<Tile> limitations_){
            self = self_;
            limitations = limitations_;
        }
        // Returns true if tested tile can be added, false otherwise
        public bool CheckLimitations(Tile test)
        {
            if(test == null) { return true; }
            if(test == self) { return false; }
            if (limitations.Contains(test)) { return false; }
            return true;
        }
        public bool CheckLimitations(string test)
        {
            if (test == "") { return true; }
            if (test == self.name) { return false; }
            foreach(Tile tile in limitations)
            {
                if (tile.name == test) { return false; }
            }
            return true;
        }
        public Tile GetSelf()
        {
            return self;
        }
    }

	// Use this for initialization
	void Start () {
        InitTiles();
        offset = (int)Random.Range(0f, 1000f);
        GenerateIsland();
        GenerateMountain(0.0015f, 0.0005f, 4);
        for (int rivers = 0; rivers < numberOfRivers; rivers++)
        {
            Vector3Int randomCoordinates = RandomizeCoordinates(centerRadius);
            GenerateRiver(randomLength, randomCoordinates.z, randomCoordinates.x, randomCoordinates.y, water);
        }
        SpawnLakes(5);
        TileRules treeRule = new TileRules(tree, new List<Tile> { water, rock, snowyRock, ground });
        TileRules oreRule = new TileRules(ore, new List<Tile> { water });
        Dictionary<string, List<SpawnPoint>> spawnPoints = GetSpawnPoints();
        GenerateNodes(treeRule, 25, spawnPoints, 9);
        GenerateNodes(oreRule, 100, spawnPoints, 1);
        PlayerState playerScript = GameObject.Find("Player").GetComponent<PlayerState>();
        playerScript.SetSelfPosition();
    }

    void InitTile(Tile tile, uint amount, float time)
    {
        tile.gameObject = Instantiate(TilePrefab);
        tile.gameObject.GetComponent<TileScript>().Init(tile.name, amount, time);
    }

    void InitTiles()
    {
        // Tile, amount, gather time
        InitTile(grass, 5, 1);
        InitTile(water, 1, 1);
        InitTile(tree, 10, 10);
        InitTile(rock, 1, 5);
        InitTile(ore, 1, 10);
    }

    void GenerateNodes(TileRules nodetype, int amount, Dictionary<string, List<SpawnPoint>> spawns, int size)
    {
        List<SpawnPoint> availableSpawns = new List<SpawnPoint>();
        foreach( string key in spawns.Keys)
        {
            if (nodetype.CheckLimitations(key)) { availableSpawns.AddRange(spawns[key]); }
        }
        for(int i = 0; i < amount; i++)
        {
            int iterator = Random.Range(0, availableSpawns.Count);
            SpawnPoint spawn = availableSpawns[iterator];
            GenerateArea(size, spawn.x, spawn.y, nodetype, true);
            availableSpawns.Remove(spawn);
        }
    }

    public Dictionary<string, List<SpawnPoint>> GetSpawnPoints()
    {
        int offset = Random.Range(0, 10000);
        Dictionary<string, List<SpawnPoint> > points = new Dictionary<string, List<SpawnPoint>>();
        for(int x = 1; x < width - 1; x++)
        {
            for(int y = 1; y < height - 1; y++)
            {
                if (IsPeak(x, y, offset, 50)) {
                    if (map.GetTile(new Vector3Int(x - width / 2, y - height / 2, 0)) != null) {
                        if (!points.ContainsKey(map.GetTile(new Vector3Int(x - width / 2, y - height / 2, 0)).name))
                        {
                            points.Add(map.GetTile(new Vector3Int(x - width / 2, y - height / 2, 0)).name, new List<SpawnPoint>());
                        }
                        points[map.GetTile(new Vector3Int(x - width / 2, y - height / 2, 0)).name].Add(
                            new SpawnPoint(x - width / 2, y - height / 2, (Tile)map.GetTile(new Vector3Int(x - width / 2, y - height / 2, 0))));
                    }
                }
            }
        }
        return points;
    }

    void SpawnLakes(int numberOfLakes)
    {
        Dictionary<string, List<SpawnPoint>> spawns = GetSpawnPoints();
        if (spawns["water"].Count == 0) { return; }
        int spawnPoint = Random.Range(0, spawns["water"].Count);
        SpawnPoint spawn = spawns["water"][spawnPoint];
        TileRules waterRule = new TileRules(water, new List<Tile>());
        for(int i = 0; i < numberOfLakes; i++)
        {
            GenerateArea(5, spawn.x+1, spawn.y, waterRule);
            GenerateArea(5, spawn.x-1, spawn.y, waterRule);
            GenerateArea(5, spawn.x, spawn.y+1, waterRule);
            GenerateArea(5, spawn.x, spawn.y-1, waterRule);
            spawns["water"].RemoveAt(spawnPoint);
            if (spawns["water"].Count == 0) { return; }
            spawnPoint = Random.Range(0, spawns["water"].Count);
            spawn = spawns["water"][spawnPoint];
        }
    }

    bool IsPeak(int x, int y, int offset, float scale)
    {
        float xCoord = (float)x / width * scale + offset;
        float yCoord = (float)y / height * scale + offset;
        float value = Mathf.PerlinNoise(xCoord, yCoord);
        if (value <= Mathf.PerlinNoise((float)(x+1) / width * scale + offset, (float)y / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)(x+1) / width * scale + offset, (float)(y+1) / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)(x+1) / width * scale + offset, (float)(y-1) / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)x / width * scale + offset, (float)(y+1) / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)x / width * scale + offset, (float)(y-1) / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)(x-1) / width * scale + offset, (float)y / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)(x-1) / width * scale + offset, (float)(y+1) / height * scale + offset)) { return false; }
        if (value <= Mathf.PerlinNoise((float)(x-1) / width * scale + offset, (float)(y-1) / height * scale + offset)) { return false; }
        return true;
    }

    Vector3Int RandomizeCoordinates(int amount)
    {
        int x = (int)(amount * Random.Range(0f, 1f));
        int y = amount - x;
        x = x * RndMinus();
        y = y * RndMinus();
        int direction = 0;
        if (y >= -centerRadius / 2 && y < centerRadius / 2) { direction = 2; }
        else if (y < -centerRadius / 2) { direction = 4; }
        if (x < -centerRadius / 2) {
            if (direction == 4) { direction += 1; }
            else if (direction == 0) { direction -= 1; }
            else { direction = 6; }
        }
        else if (x > centerRadius / 2) {
            if (direction == 4) { direction -= 1; }
            else if (direction == 0) { direction += 1; }
        }
        if (direction < 0) { direction += 8; }
        return new Vector3Int(x, y, direction);
    }

    int RndMinus()
    {
        if (Random.Range(0, 10) > 4) { return 1; }
        return -1;
    }

    bool CheckTilesAt(int x, int y, Tilemap checkedMap, TileRules tester)
    {
        for(int z = -10; z <= 10; z++)
        {
            if(checkedMap.GetTile(new Vector3Int(x,y,z)) != null)
            {
                if (!tester.CheckLimitations((Tile)checkedMap.GetTile(new Vector3Int(x, y, z)))){ return false; }
            }
        }
        return true;

    }

    void GenerateArea(int length, int x, int y, TileRules tile, bool onExisting = false)
    {
        if (Random.Range(0, 10) > 5) { length -= 1; }
        if (length <= 0) { return; }
        length -= 1;
        if (Random.Range(0, 10) <= 2) { length += 1; }

        // Create tile on current position
        if(onExisting && map.GetTile(new Vector3Int(x, y, 0)) != null)
        {
            if (CheckTilesAt(x, y, nodes, tile) && CheckTilesAt(x, y, map, tile))
            {
                if(tile.GetSelf() == water)
                {
                    map.SetTile(new Vector3Int(x, y, 0), water);
                }
                nodes.SetTile(new Vector3Int(x, y, 0), tile.GetSelf());
            }
        }
        else if (!onExisting && CheckTilesAt(x, y, nodes, tile) && CheckTilesAt(x, y, map, tile))
        {
            if (tile.GetSelf() == water)
            {
                map.SetTile(new Vector3Int(x, y, 0), water);
            }
            nodes.SetTile(new Vector3Int(x, y, 0), tile.GetSelf());
        }

        bool TilesAdded = false;
        if (y < height/2)   // north
        {
            for (int north = 1; north < length + Random.Range(0,2); north++)
            {
                if (CheckTilesAt(x, y + north, nodes, tile) && CheckTilesAt(x, y + north, map, tile) && y + north < height/2)
                {
                    if ((onExisting && map.GetTile(new Vector3Int(x, y + north, 0)) == null)){ break; }
                    TilesAdded = true;
                    if (tile.GetSelf() == water)
                    {
                        map.SetTile(new Vector3Int(x, y + north, 0), water);
                    }
                    nodes.SetTile(new Vector3Int(x, y + north, 0), tile.GetSelf());
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int west = 1; west < length + Random.Range(0, 2); west++)
            {
                if (CheckTilesAt(x - west, y, nodes, tile) && CheckTilesAt(x - west, y, map, tile) && x - west > -width/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x - west, y, 0)) == null) { break; }
                    TilesAdded = true;
                    if (tile.GetSelf() == water)
                    {
                        map.SetTile(new Vector3Int(x - west, y, 0), water);
                    }
                    nodes.SetTile(new Vector3Int(x - west, y, 0), tile.GetSelf());
                }
                else { break; }
            }
        }
        if (y > -height/2)   // south
        {
            for (int south = 1; south < length + Random.Range(0, 2); south++)
            {
                if (CheckTilesAt(x, y - south, nodes, tile) && CheckTilesAt(x, y - south, map, tile) && y - south > -height/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y - south, 0)) == null) { break; }
                    TilesAdded = true;
                    if (tile.GetSelf() == water)
                    {
                        map.SetTile(new Vector3Int(x, y - south, 0), water);
                    }
                    nodes.SetTile(new Vector3Int(x, y - south, 0), tile.GetSelf());
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int east = 1; east < length + Random.Range(0, 2); east++)
            {
                if (CheckTilesAt(x + east, y, nodes, tile) && CheckTilesAt(x + east, y, map, tile) && x + east < width/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x + east, y, 0)) == null) { break; }
                    TilesAdded = true;
                    if (tile.GetSelf() == water)
                    {
                        map.SetTile(new Vector3Int(x + east, y, 0), water);
                    }
                    nodes.SetTile(new Vector3Int(x + east, y, 0), tile.GetSelf());
                }
                else { break; }
            }
        }

        if (TilesAdded)
        {
            if (y + 1 < height/2) { GenerateArea(length, x, y + 1, tile, onExisting); }
            if (y - 1 > -height/2) { GenerateArea(length, x, y - 1, tile, onExisting); }
            if (x + 1 < width/2) { GenerateArea(length, x + 1, y, tile, onExisting); }
            if (x - 1 > -width/2) { GenerateArea(length, x - 1, y, tile, onExisting); }
        }
        return;
    }

    void GenerateMountain(float topTrigger, float midTrigger, int multiplier)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float depth = CalculateDepth(i, j, multiplier);
                if (depth > midTrigger)
                {
                    map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), null);
                    if (depth <= topTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), rock); }
                    else if (depth > topTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), snowyRock); }
                }
            }
        }
    }

    void GenerateIsland()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float depth = CalculateDepth(i, j);
                if (depth > groundTrigger && depth <= grassTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), ground); }
                if (depth > grassTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), grass); }
            }
        }
    }

    float CalculateDepth(int x, int y, int multiplier = 1)
    {
        float xCoord = (float)x / width * scale + offset;
        float yCoord = (float)y / height * scale + offset;
        float value = Mathf.PerlinNoise(xCoord, yCoord);
        float xDif, yDif;
        if (x >= width / 2) { xDif = (float)(width / 2) - (x % (width / 2) + 1); }
        else { xDif = x % (width / 2); }
        if (y >= height / 2) { yDif = (float)(height / 2) - (y % (height / 2) + 1); }
        else { yDif = y % (height / 2); }
        xDif = xDif / (width / 2);
        yDif = yDif / (height / 2);
        for (int i = 0; i < multiplier; i++)
        {
            value = (float)((xDif / 2) * (yDif / 2)) * value;
        }
        return value;
    }

    void GenerateRiver(int length, int direction, int x, int y, Tile tile)
    {
        if (nodes.GetTile(new Vector3Int(x, y, 0)) == tile ||
                (map.GetTile(new Vector3Int(x, y, 0)) == null)) {
            return;
        }
        else {
            map.SetTile(new Vector3Int(x, y, 0), water);
            nodes.SetTile(new Vector3Int(x, y, 0), tile);
        }
        for (int i = 0; i < length; i++)
        {
            int rnd = ChooseDirection(direction);
            switch (rnd)
            {
                case 0: //up
                    y++;
                    break;
                case 1: //right
                    x++;
                    break;
                case 2: //down
                    y--;
                    break;
                case 3: //left
                    x--;
                    break;
            }
            if (nodes.GetTile(new Vector3Int(x, y, 0)) == tile ||
                map.GetTile(new Vector3Int(x, y, 0)) == null)
            {
                lastDirection = -1;
                return;
            }
            else {
                map.SetTile(new Vector3Int(x, y, 0), water);
                nodes.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    //A function that generates a randomized line
    void GenerateRandomLine(int length, int Direction, int x, int y, Tile tile)
    {
        if (length < 0 || map.GetTile(new Vector3Int(x, y, 0)) == null)
        {
            return;
        }

        //The tile in the coordinates is checked, if it doesn't contain the same tile, a new tile is created and the length is reduced
        if ((nodes.GetTile(new Vector3Int(x, y, 0)) != tile))
        {
            nodes.SetTile(new Vector3Int(x, y, 0), tile);
            length--;
        }

        // check if river is within 2 tiles from the edge of the world
        if( !InRange(x-2, x+2, y-2, y+2))
        {
            if(x-2 < -width / 2) {
                nodes.SetTile(new Vector3Int(x-1, y, 0), tile);
                return;
            }
            if (x + 2 > width / 2)
            {
                nodes.SetTile(new Vector3Int(x + 1, y, 0), tile);
                return;
            }
            if (y - 2 < -height / 2)
            {
                nodes.SetTile(new Vector3Int(x, y - 1, 0), tile);
                return;
            }
            if (y + 2 > -height / 2)
            {
                nodes.SetTile(new Vector3Int(x, y + 1, 0), tile);
                return;
            }
        }
        //The direction of the next block is randomised. The main direction of the river/whatever 
        //adds weight to the direction of the next block
        int rnd = ChooseDirection(Direction);
        //Switch case on which direction the next tile will be placed
        switch (rnd)
        {
            //Cases 0, 1, 2, 3 correspond to north, east, west and south.
            //The first if checks that the next tile and the tiles adjacent to it are free. If they aren't, the function is called again with same parameters.
            //Otherwise if the next tile (and its adjacent tiles) are free, the function is called with this new position
            case 0:
                if (nodes.GetTile(new Vector3Int(x, y + 1, 0)) == tile || nodes.GetTile(new Vector3Int(x + 1, y + 1, 0)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y + 1, 0)) == tile || nodes.GetTile(new Vector3Int(x, y + 2, 0)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x, y + 1, 0)) != tile)
                {
                    GenerateRandomLine(length, Direction, x, y + 1, tile);
                }
                break;
            case 1:

                if (nodes.GetTile(new Vector3Int(x + 1, y, 0)) == tile || nodes.GetTile(new Vector3Int(x + 1, y - 1, 0)) == tile ||
                    nodes.GetTile(new Vector3Int(x + 1, y + 1, 0)) == tile || nodes.GetTile(new Vector3Int(x + 2, y, 0)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x + 1, y, 0)) != tile)
                {
                    GenerateRandomLine(length, Direction, x + 1, y, tile);
                }
                break;

            case 2:
                if (nodes.GetTile(new Vector3Int(x, y - 1, 0)) == tile || nodes.GetTile(new Vector3Int(x + 1, y - 1, 0)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y - 1, 0)) == tile || nodes.GetTile(new Vector3Int(x, y - 2, 0)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x, y - 1, 0)) != tile)
                {
                    GenerateRandomLine(length, Direction, x, y - 1, tile);
                }
                break;

            case 3:
                if (nodes.GetTile(new Vector3Int(x - 1, y, 0)) == tile || nodes.GetTile(new Vector3Int(x - 1, y - 1, 0)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y + 1, 0)) == tile || nodes.GetTile(new Vector3Int(x - 2, y, 0)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x - 1, y, 0)) != tile)
                {
                    GenerateRandomLine(length, Direction, x - 1, y, tile);
                }
                break;
        }
    }

    bool InRange(int xMin, int xMax, int yMin, int yMax)
    {
        if (xMin < -width / 2 || xMax > width / 2) { return false; }
        if (yMin < -height / 2 || yMax > height / 2) { return false; }
        return true;
    }

    int ChooseDirection(int direction)
    {
        //The direction of the line is randomised.
        //Cases 1, 3, 5 and 7 are main cardinal points (nort, east, south, west)
        //2, 4, 6 and 8 are intercardinal directions (NE, SE, SW, NW)
        //At the start a random number is generated between 0 and 5. 1-3 correspond to the cardinal directions, 
        //while 4-5 are converted into cardinal directions according to the main direction
        //int rnd = Random.Range(0, 6);
        //switch (direction)
        //{
        //    case 0:
        //        if (rnd > 3) { rnd = 0; }
        //        break;
        //    case 1:
        //        if (rnd == 4) { rnd = 0; }
        //        else if (rnd == 5) { rnd = 1; }
        //        break;
        //    case 2:
        //        if (rnd > 3) { rnd = 1; }
        //        break;
        //    case 3:
        //        if (rnd == 4) { rnd = 1; }
        //        else if (rnd == 5) { rnd = 2; }
        //        break;
        //    case 4:
        //        if (rnd > 3) { rnd = 2; }
        //        break;
        //    case 5:
        //        if (rnd == 4) { rnd = 2; }
        //        else if (rnd == 5) { rnd = 3; }
        //        break;
        //    case 6:
        //        if (rnd > 3) { rnd = 3; }
        //        break;
        //    case 7:
        //        if (rnd == 4) { rnd = 3; }
        //        else if (rnd == 5) { rnd = 0; }
        //        break;
        //}
        //if(Mathf.Abs(direction - rnd*2) > 2 && (direction + rnd * 2) % 7 > 2) {
        //    rnd = Mathf.Abs(direction + (Random.Range(-2, 1))) / 2;
        //}

        int maxValue = 8;
        int rnd = Random.Range(0, maxValue);
        int lowBound = 4;
        int highBound = 6;
        switch (direction)
        {
            case 0:
                direction = 0;
                if (rnd >= lowBound && rnd < highBound) { direction++; }
                else if(rnd >= highBound) { direction--; }
                break;
            case 1:
                if (rnd < maxValue / 2) { direction = 0; }
                else if (rnd >= maxValue / 2) { direction = 1; }
                break;
            case 2:
                direction = 1;
                if (rnd >= lowBound && rnd < highBound) { direction++; }
                else if (rnd >= highBound) { direction--; }
                break;
            case 3:
                if (rnd < maxValue / 2) { direction = 1; }
                else if (rnd >= maxValue / 2) { direction = 2; }
                break;
            case 4:
                direction = 2;
                if (rnd >= lowBound && rnd < highBound) { direction++; }
                else if (rnd >= highBound) { direction--; }
                break;
            case 5:
                if (rnd < maxValue / 2) { direction = 2; }
                else if (rnd >= maxValue / 2) { direction = 3; }
                break;
            case 6:
                direction = 3;
                if (rnd >= lowBound && rnd < highBound) { direction++; }
                else if (rnd >= highBound) { direction--; }
                break;
            case 7:
                if (rnd < maxValue / 2) { direction = 3; }
                else if (rnd >= maxValue / 2) { direction = 0; }
                break;
        }
        if(direction < 0) { direction += 4; }
        direction = direction % 4;
        if (lastDirection != -1)
        {
            if (lastDirection < 2) { if (direction == lastDirection + 2) { direction = lastDirection; } }
            else if (lastDirection - 2 == direction) { direction = lastDirection; }
        }
        lastDirection = direction;
        return direction;
    }
}

