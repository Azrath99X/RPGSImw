// File: DialogueTrigger.cs
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Tooltip("Percakapan yang akan dimulai saat berinteraksi")]
    public DialogueConversation initialConversation;

    // Ini akan dipanggil oleh skrip PlayerController Anda
    [Tooltip("saat pemain menekan Z / SPACE [cite: 39]")]
    public void Interact()
    {
        DialogueManager.Instance.StartConversation(initialConversation);
    }
}