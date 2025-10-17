using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private string npcName; // Name of the NPC
    [SerializeField] private string interactionMessage = "Hello, traveler!"; // Default interaction message

    private bool isPlayerInRange = false; // Tracks if the player is near the NPC
    private Player player; // Reference to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is the player
        player = collision.GetComponent<Player>();
        if (player)
        {
            isPlayerInRange = true; // Set the flag to true
            Debug.Log($"{npcName}: Player is in range.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the object leaving the trigger is the player
        if (collision.GetComponent<Player>() == player)
        {
            isPlayerInRange = false; // Set the flag to false
            player = null; // Clear the player reference
            Debug.Log($"{npcName}: Player left the range.");
        }
    }

    private void Update()
    {
        // Check if the player is in range and presses the "E" key
        // Also, make sure the dialogue box isn't already open
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !DialogueManager.instance.IsDialogueActive())
        {
            Interact(); // Trigger the interaction
        }
    }

    // Function to handle the interaction logic
    private void Interact()
    {
        // OLD WAY:
        // Debug.Log($"{npcName}: {interactionMessage}"); 

        // NEW WAY: Call the DialogueManager to show the dialogue box
        DialogueManager.instance.ShowDialogue(npcName, interactionMessage);
    }

}