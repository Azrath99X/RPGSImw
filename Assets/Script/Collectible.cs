using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectible : MonoBehaviour
{
    private bool isPlayerInRange = false; // Tracks if the player is in range
    private Player player; // Reference to the player in range

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger is the player
        player = collision.GetComponent<Player>();
        if (player)
        {
            isPlayerInRange = true; // Set the flag to true
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the object leaving the trigger is the player
        if (collision.GetComponent<Player>() == player)
        {
            isPlayerInRange = false; // Set the flag to false
            player = null; // Clear the player reference
        }
    }

    private void Update()
    {
        // Check if the player is in range and presses the "E" key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Item item = GetComponent<Item>();

            if (item != null && player != null)
            {
                player.inventory.add("Backpack", item); // Add the item to the player's inventory
                Destroy(this.gameObject); // Destroy the collectible object
            }
        }
    }
}