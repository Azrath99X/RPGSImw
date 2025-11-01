// File: SaveData.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // 1. Data dari TimeManager
    public int currentDay;

    // 2. Data dari StoryManager
    public int trustAir;
    public int trustXylem;

    // 3. Data dari InventoryManager
    // Kita membutuhkan class Inventory dan Slot Anda juga [System.Serializable]
    // (Berdasarkan file Anda, ini sudah benar)
    public Inventory backpackData;
    public Inventory toolbarData;

    // 4. Data dari QuestManager
    // Kita tidak bisa menyimpan ScriptableObject, jadi kita simpan ID-nya
    public List<string> activeQuestIDs;
    public List<string> completedQuestIDs;

    // 5. Data dari TileManager
    // Kita simpan setiap petak yang telah berubah
    public List<TileState> tileData;

    // Constructor untuk membuat data default saat game baru
    public SaveData()
    {
        currentDay = 1;
        trustAir = 0;
        trustXylem = 0;
        
        // Ukuran harus sesuai dengan InventoryManager Anda
        backpackData = new Inventory(20); // Ganti 20 dengan backpackSize Anda
        toolbarData = new Inventory(7);  // Ganti 7 dengan toolbarSize Anda
        
        activeQuestIDs = new List<string>();
        completedQuestIDs = new List<string>();
        
        tileData = new List<TileState>();
    }
}

// Struct kecil untuk menyimpan status petak
[System.Serializable]
public struct TileState
{
    public Vector3Int position;
    public string tileName;
}