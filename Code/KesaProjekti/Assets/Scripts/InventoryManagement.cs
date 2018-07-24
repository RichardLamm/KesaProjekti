using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagement : MonoBehaviour {

    public GridLayoutGroup inventoryGrid;
    public GridLayoutGroup toolBelt;
    public GameObject resourceSlot;
    public JsonData resourceData;

    public int inventorySize = 20;
    private int inventoryColumns;

    public bool inventoryChanged = true;
    public bool newTools = true;
    private int lastFreeSlot = 0;
    public int freeSlots;
    private List <GameObject> inventorySlots = new List<GameObject> {};

    private List<string> tools = new List<string> { "axe", "bucket", "pick", "scythe" };
    private Dictionary<string, stackData> items = new Dictionary<string, stackData>() {};
    private List<string> startingResources = new List<string> { "wood", "minerals"};

    private int highlightIndex = 0;
    private Vector4 defaultColor = new Vector4(1, 1, 1, 1);

     public class stackData
    {
        public int stackSize;
        public int amount;

        public stackData(int stack, int am)
        {
            stackSize = stack;
            amount = am;
        }
    }

    
    // Use this for initialization
    public void CreateInventorySlots () {
        for (int x = 0; x < inventorySize; x++)
        {
            GameObject slot = Instantiate(resourceSlot);
            slot.GetComponent<Image>().color = Color.clear;
            
            slot.GetComponent<RectTransform>().SetParent(inventoryGrid.transform);
            slot.SetActive(true);
            slot.transform.localScale = new Vector3(1, 1, 0);
            inventorySlots.Add(slot);
        }
        inventoryColumns = inventoryGrid.constraintCount;
        freeSlots = inventorySize;
        
    }

    public void CreateStartingInventory()
    {
        foreach (string resource in startingResources)
        {
            stackData stackInfo = new stackData(-1, -1);
            stackInfo.stackSize = resourceData.GetStackSize(resource);
            stackInfo.amount = stackInfo.stackSize;
            items[resource] = stackInfo;
        }
        Debug.Log(items.Count);
    }

	// Update is called once per frame
	void Update () {
		
	}
    public bool ValueNeedsUpdating(string itemName, int itemAmount)
    {

        var slot = inventorySlots.Find(x => x.name == itemName);
        if(slot.GetComponentInChildren<Text>().text != itemAmount.ToString())
        {

        }

        return true;
    }

    public void ShowItems(string itemName, stackData itemAmount)
    {
        //TODO:tiivistä inventoryä jos itemit tiputetaan sloteista tai niitä käytetään.

        if(inventorySlots.Find(x => x.name == itemName) != null)
        {
            var slot = inventorySlots.Find(x => x.name == itemName);
            slot.GetComponentInChildren<Text>().text = itemAmount.amount.ToString();
        }

        else if (inventorySlots[lastFreeSlot].GetComponent<Image>().sprite == null)
        {
            GameObject slot = inventorySlots[lastFreeSlot];
            slot.GetComponent<Image>().color = defaultColor;
            slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("RawResources/" + itemName);
            slot.GetComponentInChildren<Text>().text = itemAmount.amount.ToString();
            slot.name = itemName;

            lastFreeSlot++;
            freeSlots--;
        }
        else if (lastFreeSlot < inventorySize)//Käytä freeslotteja?
        {
            //Jotain, selviää kun "tiivistys" lisätty
        }
    }
    public void GetItems()
    {
        if (inventoryChanged == true) {
            foreach (var item in items)
            {
                ShowItems(item.Key, item.Value);
            }
            inventoryChanged = false;
        }
    }

    public void AddItems(string name, int newItems)
    {     
        if (items.ContainsKey(name) != false)
        {
            items[name].amount = items[name].amount + newItems;
        }
        else
        {
            stackData stackInfo = new stackData(-1, -1);
            stackInfo.stackSize = resourceData.GetStackSize(name);
            stackInfo.amount = newItems;
            items[name] = stackInfo;
        }
    }

    public void GetTools()
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


    public void MoveHighlight()
    {
        int nextSlot;
        inventorySlots[highlightIndex].transform.Find("Highlight").GetComponent<Image>().color = defaultColor;
        if (Input.GetButtonDown("Horizontal") == true)
        {
            nextSlot = highlightIndex + (int)Input.GetAxisRaw("Horizontal");
            if (nextSlot >= 0 && nextSlot < inventorySize)
            {
                inventorySlots[highlightIndex].transform.Find("Highlight").GetComponent<Image>().color = Color.clear;
                highlightIndex = nextSlot;
            }   

        }
        else if (Input.GetButtonDown("Vertical") == true)
        {
            nextSlot = highlightIndex - ((int)Input.GetAxisRaw("Vertical") * inventoryColumns);
            if (nextSlot >= 0 && nextSlot < inventorySize)
            {
                inventorySlots[highlightIndex].transform.Find("Highlight").GetComponent<Image>().color = Color.clear;
                highlightIndex = nextSlot;
            }
        }
    }
}
