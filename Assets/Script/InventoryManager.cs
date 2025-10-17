using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();
    [Header("Backpack")]
    public Inventory backpack;
    public int backpackSize;
    
    [Header("Toolbar")]
    public Inventory toolbar;
    public int toolbarSize;

    void Awake()
    {
        backpack = new Inventory(backpackSize);
        toolbar = new Inventory(toolbarSize);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);
    }

    public void add(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].add(item);
        }
    }

    public Inventory GetInventoryByName(string name)
    {
        if (inventoryByName.ContainsKey(name))
        {
            return inventoryByName[name];
        }
        return null;
    }


}
