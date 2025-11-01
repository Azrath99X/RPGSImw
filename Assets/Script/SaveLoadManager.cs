// File: SaveLoadManager.cs
using UnityEngine;
using System.IO; // <-- PENTING

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string saveFilePath;
    private SaveData currentSaveData;

    // Referensi ke semua manajer (bisa di-cache dari GameManager)
    private TimeManager timeManager;
    private StoryManager storyManager;
    private InventoryManager inventoryManager;
    private QuestManager questManager;
    private TileManager tileManager;
    private UIManager uiManager;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        // Tentukan path file save
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    void Start()
    {
        // Cache semua manajer untuk menghindari panggilan 'Instance' berulang
        CacheManagers();
    }

    private void CacheManagers()
    {
        // Asumsi semua manajer ada di GameManager atau mudah ditemukan
        timeManager = TimeManager.Instance;
        storyManager = StoryManager.Instance;
        inventoryManager = InventoryManager.Instance;
        questManager = QuestManager.Instance;
        tileManager = TileManager.Instance;
        uiManager = GameManager.Instance.uiManager; //
    }

    // Fungsi untuk memulai game baru
    public void NewGame()
    {
        currentSaveData = new SaveData();
        ApplyDataToManagers();
        Debug.Log("Memulai game baru, data default dibuat.");
    }

    public void SaveGame()
    {
        // --- PERBAIKAN DI SINI ---
        if (currentSaveData == null)
        {
            Debug.LogWarning("currentSaveData was null (Game started without New/Load?). Creating new SaveData instance to hold current state.");
            currentSaveData = new SaveData(); 
            // JANGAN panggil NewGame() atau ApplyDataToManagers()
            // Kita hanya butuh 'wadah'-nya saja.
        }
        // --- AKHIR PERBAIKAN ---
        
        // 1. Kumpulkan data TERBARU dari semua manajer
        //    (Ini sekarang akan mengambil progres Anda saat ini, bukan data reset)
        GatherDataFromManagers(); 

        // 2. Konversi ke JSON
        string json = JsonUtility.ToJson(currentSaveData, true); // 'true' untuk format cantik

        // 3. Tulis ke file
        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Game berhasil disimpan ke: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal menyimpan game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("File save tidak ditemukan. Memulai game baru.");
            NewGame();
            return;
        }

        // 1. Baca dari file
        try
        {
            string json = File.ReadAllText(saveFilePath);
            
            // 2. Konversi dari JSON
            currentSaveData = JsonUtility.FromJson<SaveData>(json);
            
            // 3. Terapkan data ke semua manajer
            ApplyDataToManagers();
            Debug.Log("Game berhasil di-load.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gagal me-load game: {e.Message}");
            NewGame(); // Buat game baru jika file save korup
        }
    }

    // Mengambil data DARI manajer KE 'currentSaveData'
    private void GatherDataFromManagers()
    {
        currentSaveData.currentDay = timeManager.CurrentDay;
        
        currentSaveData.trustAir = storyManager.trustAir;
        currentSaveData.trustXylem = storyManager.trustXylem;
        
        currentSaveData.backpackData = inventoryManager.backpack;
        currentSaveData.toolbarData = inventoryManager.toolbar;

        currentSaveData.activeQuestIDs.Clear();
        foreach(Quest q in questManager.activeQuests) { currentSaveData.activeQuestIDs.Add(q.questID); }
        
        currentSaveData.completedQuestIDs.Clear();
        foreach(Quest q in questManager.completedQuests) { currentSaveData.completedQuestIDs.Add(q.questID); }

        currentSaveData.tileData = tileManager.GetChangedTileData();
    }

    // Menerapkan data DARI 'currentSaveData' KE manajer
    private void ApplyDataToManagers()
    {
        timeManager.LoadDay(currentSaveData.currentDay);
        
        storyManager.trustAir = currentSaveData.trustAir;
        storyManager.trustXylem = currentSaveData.trustXylem;

        // --- GANTI BARIS-BARIS INI ---
        // inventoryManager.backpack = currentSaveData.backpackData;
        // inventoryManager.toolbar = currentSaveData.toolbarData;
        
        // --- DENGAN YANG INI ---
        inventoryManager.LoadInventories(currentSaveData.backpackData, currentSaveData.toolbarData);
        // --- AKHIR PERUBAHAN ---

        questManager.LoadQuestData(currentSaveData.activeQuestIDs, currentSaveData.completedQuestIDs);
        
        tileManager.LoadTileData(currentSaveData.tileData);
        
        // Terakhir, refresh UI
        // Pastikan uiManager di-cache dengan benar di CacheManagers()
        if (uiManager != null)
        {
            uiManager.RefreshAll();
        }
        
        // --- GANTI BLOK INI ---
        // if (GameManager.Instance.uiManager.GetInventoryUI("Toolbar") is Toolbar_UI toolbarUI)
        // {
        //     toolbarUI.SelectSlot(0);
        // }

        // --- DENGAN BLOK INI ---
        // Refresh toolbar secara manual untuk update selected slot
        // Kita gunakan UI.instance yang memiliki referensi langsung ke toolbarUi
        if (UI.instance != null && UI.instance.toolbarUi != null)
        {
            UI.instance.toolbarUi.SelectSlot(0); //
        }
        else
        {
            Debug.LogWarning("SaveLoadManager: Tidak dapat menemukan UI.instance.toolbarUi untuk me-refresh slot!");
        }
    }
    
    // Cek apakah ada file save
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }
}