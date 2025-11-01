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

    public void LoadInventories(Inventory backpackData, Inventory toolbarData)
    {
        // Jangan ganti referensi, tapi salin datanya
        // Pastikan ukuran slotnya cocok
        if (this.backpack.slots.Count != backpackData.slots.Count)
        {
            Debug.LogWarning("Ukuran backpack di save data tidak cocok!");
        }
        if (this.toolbar.slots.Count != toolbarData.slots.Count)
        {
            Debug.LogWarning("Ukuran toolbar di save data tidak cocok!");
        }

        // Salin slot backpack
        for (int i = 0; i < this.backpack.slots.Count; i++)
        {
            if (i < backpackData.slots.Count)
            {
                this.backpack.slots[i] = backpackData.slots[i];

                Inventory.Slot loadedSlot = this.backpack.slots[i];
            if (!loadedSlot.isEmpty)
            {
                // Cari item prefab berdasarkan nama yang tersimpan
                Item itemPrefab = GameManager.Instance.itemManager.GetItem(loadedSlot.itemName); 
                if (itemPrefab != null)
                {
                    // Set icon-nya dari prefab
                    loadedSlot.icon = itemPrefab.data.icon; 
                }
                else
                {
                    Debug.LogWarning($"Icon tidak ditemukan untuk item: {loadedSlot.itemName}");
                }
            }
            }
        }

        // Salin slot toolbar
        for (int i = 0; i < this.toolbar.slots.Count; i++)
        {
            if (i < toolbarData.slots.Count)
            {
                this.toolbar.slots[i] = toolbarData.slots[i];

                // --- LOGIKA BARU: "Re-hydrate" Icon ---
                Inventory.Slot loadedSlot = this.toolbar.slots[i];
                if (!loadedSlot.isEmpty)
                {
                    Item itemPrefab = GameManager.Instance.itemManager.GetItem(loadedSlot.itemName);
                    if (itemPrefab != null)
                    {
                        loadedSlot.icon = itemPrefab.data.icon;
                    }
                    else
                    {
                        Debug.LogWarning($"Icon tidak ditemukan untuk item: {loadedSlot.itemName}");
                    }
                }
            }
        }
            this.toolbar.SelectSlot(0);

        Debug.Log("[SaveData] Berhasil load data Inventory.");
    }
}