using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] Item; // Array to hold all collectible items
    public Dictionary <string, Item> nameToItemDict = 
        new Dictionary<string, Item> (); // Dictionary to map CollectibleType to Collectibles

    private void Awake()
    {
        foreach (Item item in Item)
        {
            addItem(item);
        }
    }


    private void addItem(Item item)
    {
        if (!nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    public Item GetItem(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            return nameToItemDict[key];
        }
        return null;
    }

    
}
