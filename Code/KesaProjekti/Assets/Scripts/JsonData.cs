﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonData : MonoBehaviour
{
    private string path = "/GameData/resources.json";
    private Dictionary<string, ResourceData> database = new Dictionary<string, ResourceData>(); 

    [System.Serializable]
    public class ResourceData
    {
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

    [System.Serializable]
    public class Player
    {

        public string playerId;
        public int playerLoc;

    }

    public void readData()
    {
        string dataPath = Application.dataPath + path;
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            ResourceData[] data = JsonHelper.FromJson<ResourceData>(json);
            //DataWrapper[] player = JsonHelper.ToJson(json);
            Debug.Log(data[0].resource);
            Debug.Log(data[1].resource);

            foreach (ResourceData dataPoint in data)
            {
                database.Add(dataPoint.resource, dataPoint);
            }

        }
        
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