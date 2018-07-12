using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScript : MonoBehaviour {

    private uint amount_ = 0;
    private float gatherTime_ = 0;
    private string resourceName_;

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

    // Returns gather time
    public float Gather()
    {
        return gatherTime_;
    }

    // Returns pair containing amount of gathered resource and it's name
    public GatherPair getGathered()
    {
        return new GatherPair(amount_, resourceName_);
    }
}
