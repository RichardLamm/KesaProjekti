using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRule : MonoBehaviour {

    public BoundsInt aroundSelf;
    public Tilemap map;
    public Tile TileAllAround;
    public Tile TileWithoutNeighbours;
    public Tile TileTop;
    public Tile TileRight;
    public Tile TileBottom;
    public Tile TileLeft;
    public Tile TileTopRight;
    public Tile TileTopLeft;
    public Tile TileBotRight;
    public Tile TileBotLeft;
    public Tile TileTopBot;
    public Tile TileLeftRight;
    public Tile TileAllButTop;
    public Tile TileAllButRight;
    public Tile TileAllButBottom;
    public Tile TileAllButLeft;
    public List<string> allowedTiles; 

    public void SetAllowedTiles(List<string> tiles)
    {
        allowedTiles = tiles;
    }

    private bool CheckAllowed(string name)
    {
        foreach(string tileName in allowedTiles)
        {
            if (name.Contains(tileName)) { return true; }
        }
        return false;
    }

    public Tile CheckNeighbours(Vector3 position, Tile self)
    {
        byte type = 0;
        Tile tempTile = self;
        string selfName = self.name;
        if (map.GetTile(new Vector3Int((int)position.x, (int)position.y + 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x, (int)position.y + 1, 0)).name)) { type += 1; } // Top
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y, 0)).name)) { type += 1; } // Right
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y, 0)).name)) { type += 1; } // Left
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x, (int)position.y - 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x, (int)position.y - 1, 0)).name)) { type += 1; } // Bot
        }

        // Most common case, so early out if this is the case
        if(type == 15)
        {
            tempTile = TileAllAround;
            tempTile.gameObject = self.gameObject;
            return tempTile;
        }
        // Early out and set self to tile without similar neighbours
        if (type == 0)
        {
            tempTile = TileWithoutNeighbours;
            tempTile.gameObject = self.gameObject;
            return tempTile;
        }

        //  /   8   \
        //  2       4
        //  \   1   /

        switch (type)
        {
            // Single neighbours
            case 1: // Only bottom
                tempTile = TileBottom;
                break;          
            case 2: // Only left
                tempTile = TileLeft;
                break;
            case 4: // Only rigth
                tempTile = TileRight;
                break;
            case 8: // Only top
                tempTile = TileTop;
                break;

            // Two neighbours
            case 5: // Bottom right
                tempTile = TileBotRight;
                break;
            case 3: // Bottom left
                tempTile = TileBotLeft;
                break;
            case 12: // Top right
                tempTile = TileTopRight;
                break;
            case 10: // Top left
                tempTile = TileTopLeft;
                break;
            case 9: // Top bottom
                tempTile = TileTopBot;
                break;
            case 6: // Right left
                tempTile = TileLeftRight;
                break;

            // Three neighbours
            case 7: // All but top
                tempTile = TileAllButTop;
                break;
            case 11: // All but right
                tempTile = TileAllButRight;
                break;
            case 14: // All but bottom
                tempTile = TileAllButBottom;
                break;
            case 13: // All but left
                tempTile = TileAllButLeft;
                break;
            case 15: // All sides
                tempTile = TileAllAround;
                break;
            default:
                break;
        } // switch(type)
        tempTile.gameObject = self.gameObject;
        return tempTile;
    } // CheckNeighbours()
}
