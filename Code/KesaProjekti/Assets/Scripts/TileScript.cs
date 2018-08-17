using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScript : MonoBehaviour {

    private int gatherTime_ = 0;
    private float spreadingInterval = 5f;
    public Tilemap map;
    public Tilemap nodes;
    private Vector3Int selfPosition;
    private List<JsonData.GainedResources> resources;

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
    public void Init(string name)
    {
        if (name == "stone" || name == "tree") // Debug
        {
            JsonData.ResourceData data = GameObject.Find("Grid").GetComponent<JsonData>().GetData(name);
            if (data == null) { return; }
            resources = new List<JsonData.GainedResources>(data.gainedResources);
            gatherTime_ = data.harvestTime;
        }
    }

    // Returns pair containing amount of gathered resource and it's name
    public List<JsonData.GainedResources> Gather()
    {
        Thread.Sleep(gatherTime_);
        return resources;
    }
}
