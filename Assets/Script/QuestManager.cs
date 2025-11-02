// File: QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;       

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public static event Action<Quest> OnQuestStarted;
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>(); // <-- TAMBAHKAN INI

    [Header("Quest Database")]
    [Tooltip("Seret SEMUA aset Questable Object ke sini")]
    public List<Quest> allQuestsInGame;


    
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void StartQuest(Quest quest)
    {
        if (quest == null) return;
        if (GetQuestStatus(quest.questID) != QuestStatus.NotStarted) return;
        
        Debug.Log("QUEST BARU DIMULAI: " + quest.questName);
        quest.isComplete = false; 
        activeQuests.Add(quest);

        if (quest.showInNotifier)
        {
            OnQuestStarted?.Invoke(quest);
        }
    }

    // Fungsi baru untuk menyelesaikan quest dengan ASET
    public void CompleteQuest(Quest quest)
    {
        if (quest == null || !activeQuests.Contains(quest)) return;

        Debug.Log("QUEST SELESAI: " + quest.questName);
        quest.isComplete = true;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
    }
    
    void Update()
    {
        // Hentikan pengecekan jika quest "GetTools" sudah tidak aktif lagi
        if (GetQuestStatus("GetTools") != QuestStatus.Active) return;

        CheckToolQuestCompletion();
    }
    
    // Fungsi baru untuk menyelesaikan quest dengan ID (untuk event eksternal)
    public void CompleteQuestByID(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            CompleteQuest(quest);
        }
    }

    // --- FUNGSI PALING PENTING UNTUK HARI INI ---
    public QuestStatus GetQuestStatus(string questID)
    {
        if (completedQuests.Exists(q => q.questID == questID))
        {
            return QuestStatus.Complete;
        }
        if (activeQuests.Exists(q => q.questID == questID))
        {
            return QuestStatus.Active;
        }
        return QuestStatus.NotStarted;
    }

    // TODO: Tambahkan fungsi untuk mengecek penyelesaian quest otomatis
    // (Misal: Panggil ini setelah item diambil)
    public void CheckToolQuestCompletion()
    {
        if (GetQuestStatus("GetTools") == QuestStatus.Active)
        {
            // Menggunakan GameManager.Instance.player.inventory untuk mengecek item
            if (GameManager.Instance != null && GameManager.Instance.player != null && GameManager.Instance.player.inventory != null)
            {
                if (GameManager.Instance.player.inventory.HasItem("Hoe") && GameManager.Instance.player.inventory.HasItem("WateringCan"))
                {
                    CompleteQuestByID("GetTools");
                }
            }
        }

    }
    
    public void CheckPlantingQuestCompletion()
    {
        // Ganti "PlantCrops" dengan ID Quest menanam Anda
        if (GetQuestStatus("PlantCrops") == QuestStatus.Active)
        {
            // Logika pengecekan
            // Untuk quest ini, kita asumsikan quest selesai
            // HANYA DENGAN MENANAM 1x.

            // Jika Anda perlu mengecek apakah 5 tanaman sudah ditanam,
            // Anda perlu melacaknya di TileManager atau PlayerStats.

            // Untuk saat ini, kita anggap menanam 1x sudah cukup.
            Debug.Log("Mengecek quest menanam... Selesai!");
            CompleteQuestByID("PlantCrops");
        }
        
        if (GetQuestStatus("D2_Task") == QuestStatus.Active)
        {
            CompleteQuestByID("D2_Task");
            Debug.Log("[QuestManager] Quest 'D2_Task' (Day 2) telah selesai!");
        }
    }


    public void LoadQuestData(List<string> activeIDs, List<string> completedIDs)
    {
        activeQuests.Clear();
        completedQuests.Clear();

        foreach (string id in activeIDs)
        {
            Quest q = allQuestsInGame.Find(quest => quest.questID == id);
            if (q != null)
            {
                q.isComplete = false; // Pastikan statusnya benar
                activeQuests.Add(q);
            }
        }

        foreach (string id in completedIDs)
        {
            Quest q = allQuestsInGame.Find(quest => quest.questID == id);
            if (q != null)
            {
                q.isComplete = true; // Pastikan statusnya benar
                completedQuests.Add(q);
            }
        }
        Debug.Log($"[SaveData] Berhasil load {activeQuests.Count} quest aktif dan {completedQuests.Count} quest selesai.");
    }
}