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

    [System.Serializable]
    public class ResourceData
    {
        public string tile;
        public string resource;
        public int stackSize;
        public int harvestTime;
    }

    [System.Serializable]
    public class MyNestedObject
    {
        public int nestedVariable1;
        public string nestedVariable2;
    }

    [System.Serializable]
    public class DataWrapper
    {
        public List<ResourceData> objects;
    }

    void ParseJsonToObject(string json)
    {
        var wrappedJsonArray = JsonUtility.FromJson<DataWrapper>(json);
    }

    

    public void readData()
    {
        string dataPath = Application.dataPath + path;
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            ResourceData[] data = JsonHelper.FromJson<ResourceData>(json);
            //DataWrapper[] player = JsonHelper.ToJson(json);

            foreach (ResourceData dataPoint in data)
            {
                database.Add(dataPoint.tile, dataPoint);
                inventoryDatabase[dataPoint.resource] = dataPoint.stackSize;

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
    public Dictionary<string, Resources[]> ReturnCraftingList()
    {
        return itemDatabase;
    }
public static class JsonHelper
{

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Tiles;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Tiles = array;
        return JsonUtility.ToJson(wrapper);
    }

    [System.Serializable]
    public class Wrapper<T>
    {
        public T[] Tiles;
    }
}
  

}