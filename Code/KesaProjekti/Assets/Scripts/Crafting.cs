using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour {

    public GridLayoutGroup craftingList;
    public GameObject craftingUi;
    public GameObject buttonPrefab;
    public JsonData itemData;
    public GameObject resourceSlot;
    private List<GameObject> resourceIcons = new List<GameObject> { };

    public int maxDifferentResources;
    private Vector4 defaultColor = new Vector4(1, 1, 1, 1);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateResourceIcons()
    {
        //This function creates the resource icons for needed resources
        for (int x = 0; x < maxDifferentResources; x++)
        {
            GameObject slot = Instantiate(resourceSlot);
            slot.GetComponent<Image>().color = Color.clear;
            var resourceBar = craftingUi.transform.Find("NeededResources");
            slot.GetComponent<RectTransform>().SetParent(resourceBar.transform);
            slot.SetActive(true);
            slot.transform.localScale = new Vector3(1, 1, 0);
            resourceIcons.Add(slot);
        }
    }
    public void InstatiateCraftingUi()
    {
        //Muuta vasemmanpuoleiset craftingjutut pois buttonista?

        //This function Instatiates the crafting UI
        CreateResourceIcons();

        //The craftable item data is retrieved from JsonData Script. A icon with the name of the item is then created for all items
        //These icons are on the left side of the crafting window
        var craftingDict = itemData.ReturnCraftingList();
        foreach(var item in craftingDict)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponentInChildren<Text>().text = item;
            button.GetComponent<RectTransform>().SetParent(craftingList.transform);
            button.transform.localScale = new Vector3(1, 1, 0);                             

        }
        //Placeholder testi, muuta omaksi "highlight funktioksi myöhemmin
        var resources = itemData.ReturnNeededResources("Axe");
        for (int x = 0; x < resources.Length; x++)
        {
            var icon = resourceIcons[x];
            icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("RawResources/" + resources[x].resource);
            icon.GetComponentInChildren<Text>().text = resources[x].amount.ToString();
            icon.name = resources[x].resource;
            icon.GetComponent<Image>().color = defaultColor;
        }

    }
}
