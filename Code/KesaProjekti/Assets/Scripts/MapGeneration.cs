using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGeneration : MonoBehaviour {

    public Tilemap map;
    public Tile grass;

	// Use this for initialization
	void Start () {
        GenerateMap(-1, Random.Range(-10, 10), Random.Range(-5, 5));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateMap(int length, int x, int y)
    {
        if(length == 0) {
            return;
        }
        else if(length == -1) {
            length = Random.Range(5, 10); }
        else
        {
            if(Random.Range(1,10) > 6) { length -= 1; }
            length -= 1;
            if(length <= 0) { return; }
        }

        // Create grass tile on current position
        map.SetTile(new Vector3Int(x, y, 0), grass);

        if (x > -13)     // not leftmost tile
        {
            if (map.GetTile(new Vector3Int(x - 1, y, 0)) != null)
            {
                if(!map.GetTile(new Vector3Int(x - 1, y, 0)).name.Contains("grass"))
                {
                    GenerateMap(length, x - 1, y);
                }
            }
            else
            {
                GenerateMap(length, x - 1, y);
            }
        }
        if (x < 12)    // not rightmost tile
        {
            if (map.GetTile(new Vector3Int(x + 1, y, 0)) != null)
            {
                if (!map.GetTile(new Vector3Int(x + 1, y, 0)).name.Contains("grass"))
                {
                    GenerateMap(length, x + 1, y);
                }
            }
            else
            {
                GenerateMap(length, x + 1, y);
            }
        }
        if (y > -5)     // not bottom tile
        {
            if (map.GetTile(new Vector3Int(x, y - 1, 0)) != null)
            {
                if (!map.GetTile(new Vector3Int(x, y - 1, 0)).name.Contains("grass"))
                {
                    GenerateMap(length, x, y - 1);
                }
            }
            else
            {
                GenerateMap(length, x, y - 1);
            }
        }
        if (y < 4)     // not topline tile
        {
            if (map.GetTile(new Vector3Int(x, y + 1, 0)))
            {
                if (!map.GetTile(new Vector3Int(x, y + 1, 0)).name.Contains("grass"))
                {
                    GenerateMap(length, x, y + 1);
                }
            }
            else
            {
                GenerateMap(length, x, y + 1);
            }
        }
    }
}
