using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagement : MonoBehaviour {

    public GridLayoutGroup inventoryGrid;
    public GridLayoutGroup toolBelt;
    public GameObject resourceSlot;

    public int inventorySize = 20;

    private bool inventoryChanged = true;
    private bool newTools = true;
    private int lastFreeSlot = 0;
    private List <GameObject> inventorySlots = new List<GameObject> {};
    private int highlightIndex = 0;
    private Vector4 defaultColor = new Vector4(1, 1, 1, 1);

    // Use this for initialization
    void Start () {
        for (int x = 0; x < inventorySize; x++)
        {
            GameObject slot = Instantiate(resourceSlot);
            slot.GetComponent<Image>().color = Color.clear;
            
            slot.GetComponent<RectTransform>().SetParent(inventoryGrid.transform);
            slot.SetActive(true);
            slot.transform.localScale = new Vector3(1, 1, 0);
            inventorySlots.Add(slot);
        }
    }
	//Idea: Luo inventoryslot prefabit startissa, päivitä niiden tietoja ajonaikana.

	// Update is called once per frame
	void Update () {
		
	}

    public void showItems(string itemName, int itemAmount)
    {
        //GameObject slot = Instantiate(resourceSlot);
        //slot.GetComponent<Image> ().sprite = Resources.Load<Sprite>("RawResources/" + itemName);
        //slot.GetComponentInChildren<Text>().text = itemAmount.ToString();
        //slot.GetComponent<RectTransform>().SetParent(inventoryGrid.transform);
        //var highlight = slot.transform.Find("Highlight").GetComponent<Image>().color;
        //highlight.a = 0f;
        //slot.SetActive(true);
        //slot.transform.localScale = new Vector3(1, 1, 0);


        //TODO:tiivistä inventoryä jos itemit tiputetaan sloteista tai niitä käytetään.
        if(inventorySlots[lastFreeSlot].GetComponent<Image>().sprite == null)
        {
            GameObject slot = inventorySlots[lastFreeSlot];
            slot.GetComponent<Image>().color = defaultColor;
            slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("RawResources/" + itemName);
            slot.GetComponentInChildren<Text>().text = itemAmount.ToString();

            lastFreeSlot++;
        }
        else if (lastFreeSlot < inventorySize)
        {
            //Jotain, selviää kun "tiivistys" lisätty
        }
    }
    public void getItems(Dictionary<string, int> items)
    {
        if (inventoryChanged == true) {
            foreach (var item in items)
            {
                showItems(item.Key, item.Value);
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
                NewImage.sprite = Resources.Load<Sprite>("Tools/" + item);
                tool.GetComponent<RectTransform>().SetParent(toolBelt.transform);
                tool.SetActive(true);
                tool.transform.localScale = new Vector3(1, 1, 0);
            }
            newTools = false;
        }
    }


    public void moveHighlight()
    {
        //inventorySlots[highlightIndex].transform.Find("Highlight").GetComponent<Image>().color = defaultColor;
        //float xAxis = Input.GetAxis("Horizontal");
        //if (xAxis > 0 && highlightIndex < inventorySize)
        //{
        //    inventorySlots[highlightIndex].transform.Find("Highlight").GetComponent<Image>().color = Color.clear;
        //    highlightIndex++;
        //}
    }
}
