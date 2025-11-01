// File: Script/UI/QuestEntry_Ui.cs
using UnityEngine;
using TMPro;

public class QuestEntry_Ui : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescriptionText;

    // Fungsi ini dipanggil oleh QuestLog_Ui
    public void Setup(Quest quest)
    {
        if (quest == null) return;
        
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.description;
    }
}