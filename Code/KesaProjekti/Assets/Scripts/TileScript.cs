using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScript : MonoBehaviour {

    private uint amount_ = 0;
    private int gatherTime_ = 0;
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
    public void Init(string name, uint amount)
    {
        if (name == "rock" || name == "tree") // Debug
        {
            JsonData.ResourceData data = GameObject.Find("Grid").GetComponent<JsonData>().GetData(name);
            if (data == null) { return; }
            resourceName_ = data.resource;
            gatherTime_ = data.harvestTime;

            // TODO: add actual amounts
            amount_ = amount;
        }
    }

    // Returns pair containing amount of gathered resource and it's name
    public GatherPair Gather(string toolName)
    {
        Thread.Sleep(gatherTime_);
        return new GatherPair(amount_, resourceName_);
    }
}
