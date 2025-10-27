// File: AutomaticDialogueTrigger.cs
using UnityEngine;

public class AutomaticDialogueTrigger : MonoBehaviour
{
    public DialogueConversation conversation;
    public bool triggerOnce = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            DialogueManager.Instance.StartConversation(conversation);
            hasTriggered = true;
        }
    }
}