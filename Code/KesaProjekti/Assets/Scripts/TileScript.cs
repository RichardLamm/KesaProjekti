using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScript : MonoBehaviour {

    private uint amount_ = 0;
    private float gatherTime_ = 0;
    private string resourceName_;
    private float spreadingInterval = 5f;
    public Tilemap map;
    public Tilemap nodes;
    private Vector3Int selfPosition;

    public struct GatherPair
    {
        public uint gatherAmount;
        public string gatherName;
        public GatherPair(uint amount, string name) {
            gatherAmount = amount;
            gatherName = name;
        }
    }

    // Store values to the class
    public void Init(string name, uint amount, float gatherTime)
    {
        resourceName_ = name;
        amount_ = amount;
        gatherTime_ = gatherTime;
    }

    // Returns pair containing amount of gathered resource and it's name
    public GatherPair Gather(string toolName)
    {
        // Get data from the data base according to the tile and tool
        // TODO: get sleepTime, amount and resourceName from the data base
        int sleepTime = 500;
        Thread.Sleep(sleepTime);
        return new GatherPair(amount_, resourceName_);
    }
}
