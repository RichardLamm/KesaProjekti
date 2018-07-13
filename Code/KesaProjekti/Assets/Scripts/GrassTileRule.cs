using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassTileRule : MonoBehaviour {

    public BoundsInt aroundSelf;
    public Tilemap map;
    public Tile GrassAllAround;
    public Tile GrassWithoutNeighbours;
    public Tile GrassTop;
    public Tile GrassRight;
    public Tile GrassBottom;
    public Tile GrassLeft;
    public Tile GrassTopRight;
    public Tile GrassTopLeft;
    public Tile GrassBotRight;
    public Tile GrassBotLeft;
    public Tile GrassTopBot;
    public Tile GrassLeftRight;
    public Tile GrassAllButTopRightCorner;
    public Tile GrassAllButTopLeftCorner;
    public Tile GrassAllButBotRightCorner;
    public Tile GrassAllButBotLeftCorner;
    public Tile GrassAllButTop;
    public Tile GrassAllButRight;
    public Tile GrassAllButBottom;
    public Tile GrassAllButLeft;

    public Tile CheckNeighbours(Vector3 position, Tile self)
    {
        byte type = 0;
        Tile tempTile = self;
        aroundSelf.position = map.WorldToCell(new Vector3(position.x-1, position.y-1, 0));
        TileBase[] tiles = map.GetTilesBlock(aroundSelf);
        // Early out and set self to tile without similar neighbours
        if(tiles.Length == 0)
        {
            tempTile = GrassWithoutNeighbours;
            tempTile.gameObject = self.gameObject;
            return tempTile;
        }
        for (int i = tiles.Length-1; i >= 0; i--)
        {
            // Skip self
            if (i == 4) { continue; }
            type <<= 1;
            if (tiles[i] != null) {
                if(tiles[i].name.Contains("grass"))
                {
                    type += 1;
                }
            }
        }

        //  32  64  128
        //  8       16
        //  1   2   4

        switch (type)
        {
            // Single neighbours
            case 0: // No neighbours
                tempTile = GrassWithoutNeighbours;
                break;
            case 255: // All tiles around are grass
                tempTile = GrassAllAround;
                break;
            case 2: // Only bottom
            case 3:
            case 6:
            case 7:
                tempTile = GrassBottom;
                break;          
            case 8: // Only left
            case 9:
            case 40:
            case 41:
                tempTile = GrassLeft;
                break;
            case 16: // Only rigth
            case 20:
            case 144:
            case 148:
                tempTile = GrassRight;
                break;
            case 64: // Only top
            case 98:
            case 192:
            case 224:
                tempTile = GrassTop;
                break;
            case 254: // All but bottom left corner
                tempTile = GrassAllButBotLeftCorner;
                break;
            case 223: // All but top left corner
                tempTile = GrassAllButTopLeftCorner;
                break;
            case 127: // All but top Right corner
                tempTile = GrassAllButTopRightCorner;
                break;
            case 251: // All but bottom right corner
                tempTile = GrassAllButBotRightCorner;
                break;

            // Three neighbours
            case 22: // Right bottom with corner
                tempTile = GrassBotRight;
                break;
            case 11: // Left bottom with corner
                tempTile = GrassBotLeft;
                break;
            case 208: // Right top with corner
                tempTile = GrassTopRight;
                break;
            case 104: // Left top with corner
                tempTile = GrassTopLeft;
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
                        tempTile = GrassBottom;
                        break;
                    case 2: // Only left
                        tempTile = GrassLeft;
                        break;
                    case 4: // Only right
                        tempTile = GrassRight;
                        break;
                    case 8: // Only top
                        tempTile = GrassTop;
                        break;
                    case 5: // Bottom right
                        tempTile = GrassBotRight;
                        break;
                    case 3: // Bottom left
                        tempTile = GrassBotLeft;
                        break;
                    case 12: // Top right
                        tempTile = GrassTopRight;
                        break;
                    case 10: // Top left
                        tempTile = GrassTopLeft;
                        break;
                    case 9: // Top bottom
                        tempTile = GrassTopBot;
                        break;
                    case 6: // Right left
                        tempTile = GrassLeftRight;
                        break;
                    case 7: // All but top
                        tempTile = GrassAllButTop;
                        break;
                    case 11: // All but right
                        tempTile = GrassAllButRight;
                        break;
                    case 14: // All but bottom
                        tempTile = GrassAllButBottom;
                        break;
                    case 13: // All but left
                        tempTile = GrassAllButLeft;
                        break;
                    case 15: // All sides
                        tempTile = GrassAllAround;
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
