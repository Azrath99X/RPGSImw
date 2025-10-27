// File: QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>(); // <-- TAMBAHKAN INI
    
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
        quest.isComplete = false; // Pastikan disetel ulang
        activeQuests.Add(quest);
        // Panggil UI Quest Log di sini
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
}