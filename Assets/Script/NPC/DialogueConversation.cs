// File: DialogueConversation.cs
using UnityEngine;


// Ini bukan ScriptableObject, hanya kelas serializable
// untuk menyimpan satu baris dialog.
[System.Serializable]
public class DialogueLine
{
    public Character character; // Siapa yang berbicara
    [TextArea(3, 10)]
    public string line; // Apa yang mereka katakan 
}

[System.Serializable]
public class PlayerChoice
{
    [Tooltip("Teks yang muncul di tombol pilihan")]
    public string choiceText;
    
    [Tooltip("Percakapan yang akan dimulai jika pilihan ini diambil")]
    public DialogueConversation nextConversation;

    // --- HAPUS INI ---
    // public UnityEvent onChoiceSelected;

    // --- TAMBAHKAN INI ---
    [Header("Choice Triggers")]
    public Quest questToStartOnSelect;
    
    [Tooltip("e.g., 'Air' or 'Xylem'")]
    public string characterToTrust;
    
    [Tooltip("Amount to add (bisa negatif)")]
    public int trustAmount;
}


[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
public class DialogueConversation : ScriptableObject
{
    [Header("Dialogue Lines")]
    public DialogueLine[] lines; 

    [Header("Choices")]
    public PlayerChoice[] playerChoices; 

    [Header("End of Conversation")]
    [Tooltip("Percakapan berikutnya jika tidak ada pilihan (linear)")]
    public DialogueConversation nextConversationOnEnd;
    
    // --- HAPUS INI ---
    // public UnityEvent onConversationEnd;

    // --- TAMBAHKAN INI ---
    [Header("End Triggers")]
    public Quest questToStartOnEnd;
    public ItemData itemToGiveOnEnd; // Pastikan 'Item' adalah ScriptableObject
    public int itemGiveAmount = 1; // Opsional
}