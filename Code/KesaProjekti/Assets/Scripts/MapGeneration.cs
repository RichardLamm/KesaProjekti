﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGeneration : MonoBehaviour {

    public Tilemap map;
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
    }
	
	// Update is called once per frame
	void Update () {

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
                    if (onExisting && map.GetTile(new Vector3Int(x, y + north, 0)) == null){ break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y + north, 0), tile);
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int west = 1; west < length + Random.Range(0, 2); west++)
            {
                if (map.GetTile(new Vector3Int(x - west, y, 0)) != tile && x - west > -width/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x - west, y, 0)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x - west, y, 0), tile);
                }
                else { break; }
            }
        }
        if (y > -height/2)   // south
        {
            for (int south = 1; south < length + Random.Range(0, 2); south++)
            {
                if (map.GetTile(new Vector3Int(x, y - south, 0)) != tile && y - south > -height/2)
                {
                    if (onExisting && map.GetTile(new Vector3Int(x, y - south, 0)) == null) { break; }
                    TilesAdded = true;
                    map.SetTile(new Vector3Int(x, y - south, 0), tile);
                }
                else { break; }
            }
        }
        if (x > -width/2)     // west
        {
            for (int east = 1; east < length + Random.Range(0, 2); east++)
            {
                if (map.GetTile(new Vector3Int(x + east, y, 0)) != tile && x - east < width/2)
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
                if (depth <= groundTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), water); }
                if (depth > groundTrigger) { map.SetTile(new Vector3Int(i - width / 2, j - height / 2, 0), ground); }
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
}
