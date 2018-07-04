using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagement : MonoBehaviour {

    public GridLayoutGroup inventoryGrid;
    private bool inventoryChanged = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showItems(string itemName)
    {
        GameObject test = new GameObject();
        Image NewImage = test.AddComponent<Image>();
        //NewImage.sprite = pick;
        NewImage.sprite = Resources.Load<Sprite>("RawResources/" + itemName);
        test.GetComponent<RectTransform>().SetParent(inventoryGrid.transform);
        test.SetActive(true);
        test.transform.localScale = new Vector3(1, 1, 0);
    }
    public void getItems(Dictionary<string, int> items)
    {
        if (inventoryChanged == false) {
            foreach (var item in items)
            {
                showItems(item.Key);
            }
            inventoryChanged = true;
        }
    }
}
