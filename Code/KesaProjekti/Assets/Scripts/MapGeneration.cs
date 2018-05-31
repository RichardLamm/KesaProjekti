using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGeneration : MonoBehaviour {

    public Tilemap map;
    public Tile grass;
    public Tile ground;
    private int minX = -50;
    private int maxX = 49;
    private int minY = -25;
    private int maxY = 24;
    private enum direction {none, north, northeast, east, southeast, south, southwest, west, northwest};

	// Use this for initialization
	void Start () {
        GenerateMap(12, 10, 10, ground);
        GenerateMap(12, -10, 10, ground);
        GenerateMap(12, 10, -10, ground);
        GenerateMap(12, -10, -10, ground);
        GenerateMap(20, 0, 0, ground);
        for (int i = 0; i < Random.Range(7, 15); i++)
        {
            int tempX = Random.Range(minX, maxX);
            int tempY = Random.Range(minY, maxY);
            GenerateMap(Random.Range(5, 15), tempX, tempY, grass, true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateMap(int length, int x, int y, Tile tile, bool onExisting = false)
    {
        Debug.Log(length);
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

        if (y < maxY)   // north
        {
            for (int north = 1; north < length + Random.Range(0,2); north++)
            {
                if (map.GetTile(new Vector3Int(x, y + north, 0)) != tile && y + north < maxY)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y + north, 0)) == null){ break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y + north, 0), tile);
                }
                else { break; }
            }
        }
        if (x > minX)     // west
        {
            for (int west = 1; west < length + Random.Range(0, 2); west++)
            {
                if (map.GetTile(new Vector3Int(x - west, y, 0)) != tile && x - west > minX)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x - west, y, 0)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x - west, y, 0), tile);
                }
                else { break; }
            }
        }
        if (y > minY)   // south
        {
            for (int south = 1; south < length + Random.Range(0, 2); south++)
            {
                if (map.GetTile(new Vector3Int(x, y - south, 0)) != tile && y - south > minY)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y - south, 0)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y - south, 0), tile);
                }
                else { break; }
            }
        }
        if (x > minX)     // west
        {
            for (int east = 1; east < length + Random.Range(0, 2); east++)
            {
                if (map.GetTile(new Vector3Int(x + east, y, 0)) != tile && x - east < maxX)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x + east, y, 0)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x + east, y, 0), tile);
                }
                else { break; }
            }
        }

        if (TilesAdded)
        {
            if (y < maxY) { GenerateMap(length, x, y - 1, tile, onExisting); }
            if (y > minY) { GenerateMap(length, x, y + 1, tile, onExisting); }
            if (x < maxX) { GenerateMap(length, x - 1, y, tile, onExisting); }
            if (x < minX) { GenerateMap(length, x + 1, y, tile, onExisting); }
        }
    }
}
