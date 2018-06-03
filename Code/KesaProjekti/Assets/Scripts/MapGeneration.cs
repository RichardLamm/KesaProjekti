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
    public int width = 640;
    public int height = 480;
    public float scale = 5f;
    public float groundTrigger = 0.1f;
    public float grassTrigger = 0.3f;
    public int offset = 0;
    private enum direction {none, north, northeast, east, southeast, south, southwest, west, northwest};
    public int randomLength = 30;

	// Use this for initialization
	void Start () {
        offset = (int)Random.Range(0f, 10f) * width;
        GenerateIsland();
        //for (int i = 0; i < Random.Range(7, 15); i++)
        //{
        //    int tempX = Random.Range(-width/2, width/2);
        //    int tempY = Random.Range(-height/2, height/2);
        //    GenerateMap(Random.Range(5, 15), tempX, tempY, grass, true);
        //}
        int initialDirection = Random.Range(0, 8);
        GenerateRandomLine(randomLength, initialDirection, 0, 0, water);
    }
	
    void GenerateMap(int length, int x, int y, Tile tile, bool onExisting = false)
    {
        if (Random.Range(1, 10) > 5) { length -= 1; }
        length -= 1;
        if (length <= 0) { return; }

        // Create tile on current position
        if (!onExisting && map.GetTile(new Vector3Int(x, y, 0)) != tile)
        {
            map.SetTile(new Vector3Int(x, y, 0), tile);
        }
        else if(onExisting && map.GetTile(new Vector3Int(x, y, 0)) != null)
        {
            map.SetTile(new Vector3Int(x, y, 0), tile);
        }

        bool TilesAdded = false;

        if (y < height/2)   // north
        {
            for (int north = 1; north < length + Random.Range(0,2); north++)
            {
                if (map.GetTile(new Vector3Int(x, y + north, 0)) != tile && y + north < height/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y + north, 9)) == null){ break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y + north, 9), tile);
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int west = 1; west < length + Random.Range(0, 2); west++)
            {
                if (map.GetTile(new Vector3Int(x - west, y, 9)) != tile && x - west > -width/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x - west, y, 9)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x - west, y, 9), tile);
                }
                else { break; }
            }
        }
        if (y > -height/2)   // south
        {
            for (int south = 1; south < length + Random.Range(0, 2); south++)
            {
                if (map.GetTile(new Vector3Int(x, y - south, 9)) != tile && y - south > -height/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y - south, 9)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y - south, 9), tile);
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int east = 1; east < length + Random.Range(0, 2); east++)
            {
                if (map.GetTile(new Vector3Int(x + east, y, 9)) != tile && x - east < width/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x + east, y, 9)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x + east, y, 9), tile);
                }
                else { break; }
            }
        }

        if (TilesAdded)
        {
            if (y < height/2) { GenerateMap(length, x, y - 1, tile, onExisting); }
            if (y > -height/2) { GenerateMap(length, x, y + 1, tile, onExisting); }
            if (x < width/2) { GenerateMap(length, x - 1, y, tile, onExisting); }
            if (x < -width/2) { GenerateMap(length, x + 1, y, tile, onExisting); }
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

    float CalculateDepth(int x, int y)
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
        value = (float)((xDif / 2) * (yDif / 2)) * value;
        //Debug.Log("(" + x + "," + y + ") " + xDif + " _ " + yDif + " _ " + value);
        return value;
    }

    //A function that generates a randomized line
    void GenerateRandomLine(int length, int Direction, int x, int y, Tile tile)
    {
        if (length < 0 || map.GetTile(new Vector3Int(x, y, 0)) == null)
        {
            return;
        }

        //The tile in the coordinates is checked, if it doesn't contain the same tile, a new tile is created and the length is reduced
        if ((nodes.GetTile(new Vector3Int(x, y, 1)) != tile))
        {
            nodes.SetTile(new Vector3Int(x, y, 1), tile);
            length--;
        }

        // check if river is within 2 tiles from the edge of the world
        if( !InRange(x-2, x+2, y-2, y+2))
        {
            if(x-2 < -width / 2) {
                nodes.SetTile(new Vector3Int(x-1, y, 1), tile);
                return;
            }
            if (x + 2 > width / 2)
            {
                nodes.SetTile(new Vector3Int(x + 1, y, 1), tile);
                return;
            }
            if (y - 2 < -height / 2)
            {
                nodes.SetTile(new Vector3Int(x, y - 1, 1), tile);
                return;
            }
            if (y + 2 > -height / 2)
            {
                nodes.SetTile(new Vector3Int(x, y + 1, 1), tile);
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
                if (nodes.GetTile(new Vector3Int(x, y + 1, 1)) == tile || nodes.GetTile(new Vector3Int(x + 1, y + 1, 1)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y + 1, 1)) == tile || nodes.GetTile(new Vector3Int(x, y + 2, 1)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x, y + 1, 1)) != tile)
                {
                    GenerateRandomLine(length, Direction, x, y + 1, tile);
                }
                break;
            case 1:

                if (nodes.GetTile(new Vector3Int(x + 1, y, 1)) == tile || nodes.GetTile(new Vector3Int(x + 1, y - 1, 1)) == tile ||
                    nodes.GetTile(new Vector3Int(x + 1, y + 1, 1)) == tile || nodes.GetTile(new Vector3Int(x + 2, y, 1)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x + 1, y, 1)) != tile)
                {
                    GenerateRandomLine(length, Direction, x + 1, y, tile);
                }
                break;

            case 2:
                if (nodes.GetTile(new Vector3Int(x, y - 1, 1)) == tile || nodes.GetTile(new Vector3Int(x + 1, y - 1, 1)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y - 1, 1)) == tile || nodes.GetTile(new Vector3Int(x, y - 2, 1)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x, y - 1, 1)) != tile)
                {
                    GenerateRandomLine(length, Direction, x, y - 1, tile);
                }
                break;

            case 3:
                if (nodes.GetTile(new Vector3Int(x - 1, y, 1)) == tile || nodes.GetTile(new Vector3Int(x - 1, y - 1, 1)) == tile ||
                    nodes.GetTile(new Vector3Int(x - 1, y + 1, 1)) == tile || nodes.GetTile(new Vector3Int(x - 2, y, 1)) == tile)
                {
                    GenerateRandomLine(length, Direction, x, y, tile);
                }
                else if (nodes.GetTile(new Vector3Int(x - 1, y, 1)) != tile)
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
        int rnd = Random.Range(0, 6);
        switch (direction)
        {
            case 0:
                if (rnd > 3) { rnd = 0; }
                break;
            case 1:
                if (rnd == 4) { rnd = 0; }
                else if (rnd == 5) { rnd = 1; }
                break;
            case 2:
                if (rnd > 3) { rnd = 1; }
                break;
            case 3:
                if (rnd == 4) { rnd = 1; }
                else if (rnd == 5) { rnd = 2; }
                break;
            case 4:
                if (rnd > 3) { rnd = 2; }
                break;
            case 5:
                if (rnd == 4) { rnd = 2; }
                else if (rnd == 5) { rnd = 3; }
                break;
            case 6:
                if (rnd > 3) { rnd = 3; }
                break;
            case 7:
                if (rnd == 4) { rnd = 3; }
                else if (rnd == 5) { rnd = 0; }
                break;
        }
        if(Mathf.Abs(direction - rnd*2) > 2 && (direction + rnd * 2) % 7 > 2) {
            rnd = Mathf.Abs(direction + (Random.Range(-3, 2))) % 4;
        }
       
        return rnd;
    }
}

