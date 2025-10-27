using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // --- TAMBAHKAN SINGLETON ---
    public static InventoryManager Instance { get; private set; }
    // -------------------------

    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();
    [Header("Backpack")]
    public Inventory backpack;
    public int backpackSize;
    
    [Header("Toolbar")]
    public Inventory toolbar;
    public int toolbarSize;

    void Awake()
    {
        // --- LOGIKA SINGLETON ---
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
        // -------------------------

        backpack = new Inventory(backpackSize);
        toolbar = new Inventory(toolbarSize);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);
    }

    // Fungsi 'add' lama Anda
    public void add(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].add(item);
        }

        if (QuestManager.Instance != null)
    {
        QuestManager.Instance.CheckToolQuestCompletion();
    }
    }

    public void AddItem(Item item, int amount)
{
    if (item == null) return;

    for (int i = 0; i < amount; i++)
    {
        add("Toolbar", item); // Menambahkan ke inventaris internal
    }
    
    Debug.Log($"Menambahkan {amount} {item.data.itemName} ke inventaris!"); // Menggunakan .data

    // --- TAMBAHKAN INI ---
    // Panggil QuestManager untuk mengecek apakah quest selesai
    // Kita cek null untuk jaga-jaga jika QuestManager belum ada
    if (QuestManager.Instance != null)
    {
        QuestManager.Instance.CheckToolQuestCompletion();
    }
    // -------------------

    // Panggil fungsi refresh UI Anda di sini jika ada
}

// --- UBAH FUNGSI INI ---
public bool HasItem(string itemName)
{
    // Hapus referensi GameManager. Cek inventaris milik 'this' (Instance)
    foreach (var inventory in inventoryByName.Values)
    {
        // (Ini akan secara otomatis mengecek "Toolbar" dan "Backpack" milik Instance ini)
        foreach (var slot in inventory.slots)
        {
            if (slot.itemName == itemName)
            {
                return true; // Ditemukan!
            }
        }
    }

    // Tidak ditemukan di inventaris manapun
    return false;
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