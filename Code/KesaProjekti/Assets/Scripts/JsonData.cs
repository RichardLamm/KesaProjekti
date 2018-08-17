using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonData : MonoBehaviour
{
    private string path = "/GameData/resources.json";
    private Dictionary<string, ResourceData> database = new Dictionary<string, ResourceData>();
    private Dictionary<string, int> inventoryDatabase = new Dictionary<string, int>();
    private Dictionary<string, Resources[]> itemDatabase = new Dictionary<string, Resources[]>();

    //Three classes for resource parsing
    [System.Serializable]
    public class TileList
    {
        public ResourceData[] Tiles;
    }

    [System.Serializable]
    public class GainedResources
    {
        public string resource;
        public int gatherChance;
        public int gatherAmountMax;
        public int gatherAmountMin;
        public int stackSize;
    }

    [System.Serializable]
    public class ResourceData
    {
        public string tile;
        public int harvestTime;
        public GainedResources[] gainedResources;
    }

    public ResourceData GetData(string key)
    {
        return database[key];
    }

    public void readData()
    {
        string dataPath = Application.dataPath + path;
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            var tileList = JsonUtility.FromJson<TileList>(json);

            foreach (ResourceData dataPoint in tileList.Tiles)
            {
                database.Add(dataPoint.tile, dataPoint);
                foreach(GainedResources resource in dataPoint.gainedResources)
                {
                    inventoryDatabase[resource.resource] = resource.stackSize;
                   
                }

            } 
            readCraftingData();

        }
        

    }
    public void readCraftingData()
    {
        string dataPath = Application.dataPath + "/GameData/crafting.json";
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            var itemList = JsonUtility.FromJson<CraftableItems>("{\"Items\":" + json + "}");
            foreach(ItemList item in itemList.Items)
            {
                itemDatabase[item.item] = item.resourcesNeeded;
            }
            //Debug.Log(itemDatabase["axe"][0].resource);
        }
    }

    //Three classes for crafting item parsing
    [System.Serializable]
    public class CraftableItems
    {
        public ItemList[] Items;
    }

    [System.Serializable]
    public class Resources
    {
        public string resource;
        public int amount;
    }

    [System.Serializable]
    public class ItemList
    {
        public string item;
        public Resources[] resourcesNeeded;
    }



    public int GetStackSize(string resource)
    {
        if (inventoryDatabase.ContainsKey(resource))
        {
            return inventoryDatabase[resource];
        }
        else
        {
            
            return -1;
        }
    }
    public Dictionary<string, Resources[]>.KeyCollection ReturnCraftingList()
    {
        return itemDatabase.Keys;
    }

    public Resources[] ReturnNeededResources(string item)
    {
        return itemDatabase[item];
    }
  

}