using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public InventoryManager inventory;
    public TileManager tileManager;
    public float interactionRange = 2f; // Range within which the player can interact
    private IInteractable currentInteractable; // Reference to the current interactable object

    private void Start()
    {
        tileManager = GameManager.Instance.tileManager;
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(tileManager != null)
            {
                Vector3Int position = new Vector3Int((int)transform.position.x,
                (int)transform.position.y, 0);

                string tileName = tileManager.GetTileName(position);

                if (!string.IsNullOrEmpty(tileName))
                {
                    if (tileName == "Interactable_" && inventory.toolbar.selectedSlot.itemName == "Hoe")
                    {
                        animator.SetTrigger("IsPlowing");
                        tileManager.evolveTile(position);
                        
                    }
                }
            }
                
        }
            
        

        // Check for the "F" key press
        if (Input.GetKeyDown(KeyCode.F))
        {
            Transform closest = null;
            float ClosestDistance = Mathf.Infinity;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);

            foreach (var target in colliders)
            {
                IInteractable interactable = target.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float distance = Vector2.Distance(transform.position, target.transform.position);
                    if (distance < ClosestDistance)
                    {
                        ClosestDistance = distance;
                        closest = target.transform;
                    }
                }
            }
            if (closest == null)
            {
                return;
            }

            closest.GetComponent<IInteractable>().Interact(); // Call the Interact method on the interactable object
        }
    }

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
    }

    public void dropItem(Item item)
    {
        Vector2 spawnLocation = transform.position;

        Vector2 spawnOffset = Random.insideUnitCircle * 1f;

        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset,
            Quaternion.identity);

        
    }

    public void dropItem(Item item, int numToDrop)  
    {
        for(int i = 0; i < numToDrop; i++)
        {
            dropItem(item);
        }
    }
}


//    private void DetectInteractable()
//    {
//        Transform closest = null;
//        float ClosestDistance = Mathf.Infinity;
//        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);

//        foreach(var target in colliders)
//        {
//            IInteractable interactable = target.GetComponent<IInteractable>();
//            if (interactable != null)
//            {
//                float distance = Vector2.Distance(transform.position, target.transform.position);
//                if (distance < ClosestDistance)
//                {
//                    ClosestDistance = distance;
//                    closest = target.transform;
//                }
//            }
//        }
//        if (closest == null)
//        {
//            return;
//        }

//        closest.GetComponent<IInteractable>().Interact(); // Call the Interact method on the interactable object
//    }
//}

