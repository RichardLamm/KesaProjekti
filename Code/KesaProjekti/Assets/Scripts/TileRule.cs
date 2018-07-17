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
    public Tile TileAllButTopRightCorner;
    public Tile TileAllButTopLeftCorner;
    public Tile TileAllButBotRightCorner;
    public Tile TileAllButBotLeftCorner;
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
        if (map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y + 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y + 1, 0)).name)) { type += 1; } // Top right
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x, (int)position.y + 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x, (int)position.y + 1, 0)).name)) { type += 1; } // Top
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y + 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y + 1, 0)).name)) { type += 1; } // Top left
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
        if (map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y - 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x + 1, (int)position.y - 1, 0)).name)) { type += 1; } // Bot right
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x, (int)position.y - 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x, (int)position.y - 1, 0)).name)) { type += 1; } // Bot
        }
        type <<= 1;
        if (map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y - 1, 0)) != null)
        {
            if (CheckAllowed(map.GetTile(new Vector3Int((int)position.x - 1, (int)position.y - 1, 0)).name)) { type += 1; } // Bot left
        }

        // Most common case, so early out if this is the case
        if(type == 255)
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

        //  32  64  128
        //  8       16
        //  1   2   4

        switch (type)
        {
            // Single neighbours
            case 2: // Only bottom
            case 3:
            case 6:
            case 7:
                tempTile = TileBottom;
                break;          
            case 8: // Only left
            case 9:
            case 40:
            case 41:
                tempTile = TileLeft;
                break;
            case 16: // Only rigth
            case 20:
            case 144:
            case 148:
                tempTile = TileRight;
                break;
            case 64: // Only top
            case 98:
            case 192:
            case 224:
                tempTile = TileTop;
                break;
            case 254: // All but bottom left corner
                tempTile = TileAllButBotLeftCorner;
                break;
            case 223: // All but top left corner
                tempTile = TileAllButTopLeftCorner;
                break;
            case 127: // All but top Right corner
                tempTile = TileAllButTopRightCorner;
                break;
            case 251: // All but bottom right corner
                tempTile = TileAllButBotRightCorner;
                break;

            // Three neighbours
            case 22: // Right bottom with corner
                tempTile = TileBotRight;
                break;
            case 11: // Left bottom with corner
                tempTile = TileBotLeft;
                break;
            case 208: // Right top with corner
                tempTile = TileTopRight;
                break;
            case 104: // Left top with corner
                tempTile = TileTopLeft;
                break;

            default:
                byte sides = 0;
                sides += (byte)((type >> 6) % 2);
                sides <<= 1;
                sides += (byte)((type >> 4) % 2);
                sides <<= 1;
                sides += (byte)((type >> 3) % 2);
                sides <<= 1;
                sides += (byte)((type >> 1) % 2);
                switch (sides)
                {
                    case 1: // Only bottom
                        tempTile = TileBottom;
                        break;
                    case 2: // Only left
                        tempTile = TileLeft;
                        break;
                    case 4: // Only right
                        tempTile = TileRight;
                        break;
                    case 8: // Only top
                        tempTile = TileTop;
                        break;
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
                } // switch(sides)
                break;
        } // switch(type)
        tempTile.gameObject = self.gameObject;
        return tempTile;
    } // CheckNeighbours()
}
