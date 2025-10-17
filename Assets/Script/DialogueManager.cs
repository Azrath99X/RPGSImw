using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add this line to use TextMeshPro

public class DialogueManager : MonoBehaviour
{
    // Singleton instance
    public static DialogueManager instance;

    // UI elements
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text messageText;

    private bool isDialogueActive = false;

    private void Awake()
    {
        // Ensure there is only one instance of the DialogueManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Close dialogue box with the same key used to open it
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            HideDialogue();
        }
    }

    public void ShowDialogue(string name, string message)
    {
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        nameText.text = name;
        messageText.text = message;
    }

    public void HideDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
    }

    // Public method to check if dialogue is active
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}