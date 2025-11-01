// File: Script/UI/QuestLog_Ui.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog_Ui : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Panel utama yang akan di-toggle")]
    public GameObject questLogPanel;
    
    [Tooltip("Prefab QuestEntry_Ui")]
    public GameObject questEntryPrefab;
    
    [Tooltip("Parent object (e.g., Vertical Layout Group) tempat quest akan di-spawn")]
    public Transform contentContainer;

    // Melacak entri yang sudah di-spawn agar bisa dibersihkan
    private List<GameObject> spawnedEntries = new List<GameObject>();

    void Start()
    {
        // Mulai dalam keadaan tersembunyi
        questLogPanel.SetActive(false);
    }

    void Update()
    {
        // Toggle panel dengan tombol 'J' (Journal)
        if (Input.GetKeyDown(KeyCode.J))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        // Cek juga agar tidak bisa dibuka saat dialog atau eksplorasi
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;
        if (ExplorationModeManager.Instance != null && ExplorationModeManager.Instance.IsExploring) return;

        bool isActive = !questLogPanel.activeSelf;
        questLogPanel.SetActive(isActive);

        // Jika panel baru saja diaktifkan, refresh isinya
        if (isActive)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        // 1. Hapus semua entri quest yang lama
        foreach (GameObject entry in spawnedEntries)
        {
            Destroy(entry);
        }
        spawnedEntries.Clear();

        // 2. Cek null safety
        if (QuestManager.Instance == null)
        {
            Debug.LogError("QuestLog_Ui: QuestManager.Instance tidak ditemukan!");
            return;
        }

        // 3. Buat entri baru untuk setiap quest aktif
        foreach (Quest activeQuest in QuestManager.Instance.activeQuests)
        {
            GameObject newEntry = Instantiate(questEntryPrefab, contentContainer);
            newEntry.GetComponent<QuestEntry_Ui>().Setup(activeQuest);
            spawnedEntries.Add(newEntry);
        }
    }
}