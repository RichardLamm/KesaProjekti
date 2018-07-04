using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagement : MonoBehaviour {

    public GridLayoutGroup inventoryGrid;
    public GridLayoutGroup toolBelt;

    private bool inventoryChanged = true;
    private bool newTools = true;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showItems(string itemName)
    {
        GameObject item = new GameObject();
        Image NewImage = item.AddComponent<Image>();
        //NewImage.sprite = pick;
        NewImage.sprite = Resources.Load<Sprite>("RawResources/" + itemName);
        item.GetComponent<RectTransform>().SetParent(inventoryGrid.transform);
        item.SetActive(true);
        item.transform.localScale = new Vector3(1, 1, 0);
    }
    public void getItems(Dictionary<string, int> items)
    {
        if (inventoryChanged == true) {
            foreach (var item in items)
            {
                showItems(item.Key);
            }
            inventoryChanged = false;
        }
    }

    public void getTools(List<string> tools)
    {
        if (newTools == true)
        {
            foreach (var item in tools)
            {
                GameObject tool = new GameObject();
                Image NewImage = tool.AddComponent<Image>();
                //NewImage.sprite = pick;
                NewImage.sprite = Resources.Load<Sprite>("Tools/" + item);
                tool.GetComponent<RectTransform>().SetParent(toolBelt.transform);
                tool.SetActive(true);
                tool.transform.localScale = new Vector3(1, 1, 0);
            }
            newTools = false;
        }
    }
}
