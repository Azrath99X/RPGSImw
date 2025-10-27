// File: ConditionalDialogueTrigger.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum untuk status quest
public enum QuestStatus { NotStarted, Active, Complete }

[System.Serializable]
public class DialogueState
{
    [Tooltip("ID Quest yang akan dicek. Harus sama persis dengan ID di aset Quest.")]
    public string questID;
    [Tooltip("Status yang diperlukan untuk memutar percakapan ini.")]
    public QuestStatus requiredStatus;
    [Tooltip("Percakapan yang akan diputar jika kondisi terpenuhi.")]
    public DialogueConversation conversation;
}

public class ConditionalDialogueTrigger : MonoBehaviour, IInteractable
{
    [Tooltip("Daftar kondisi, diurutkan dari prioritas TERTINGGI ke TERENDAH.")]
    public List<DialogueState> states = new List<DialogueState>();
    
    [Tooltip("Percakapan yang akan diputar jika tidak ada kondisi quest yang terpenuhi.")]
    public DialogueConversation defaultConversation;

    public void Interact()
    {
        // Cek semua kondisi dari atas ke bawah
        foreach (var state in states)
        {
            if (string.IsNullOrEmpty(state.questID)) continue;

            // Dapatkan status quest saat ini dari manager
            QuestStatus currentStatus = QuestManager.Instance.GetQuestStatus(state.questID);

            // Jika statusnya cocok, mulai percakapan dan hentikan
            if (currentStatus == state.requiredStatus)
            {
                DialogueManager.Instance.StartConversation(state.conversation);
                return;
            }
        }
        
        // Jika tidak ada kondisi yang cocok, putar percakapan default
        DialogueManager.Instance.StartConversation(defaultConversation);
    }
}