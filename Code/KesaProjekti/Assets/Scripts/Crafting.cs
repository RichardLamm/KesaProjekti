using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour {

    public GridLayoutGroup craftingList;
    public GameObject buttonPrefab;
    public JsonData itemData;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void CreateCraftingList()
    {
        var craftingDict = itemData.ReturnCraftingList();
        foreach(var item in craftingDict)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponentInChildren<Text>().text = item;
            button.GetComponent<RectTransform>().SetParent(craftingList.transform);
            button.transform.localScale = new Vector3(1, 1, 0);
        }

        
    }
}
