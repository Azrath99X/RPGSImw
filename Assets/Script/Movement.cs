using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed; // Base movement speed
    public float sprintMultiplier = 1.5f; // Multiplier for sprinting speed
    public Animator animator;

    private Vector3 direction;
    private Vector3 lastDirection;
    private bool isSprinting; // Tracks whether the player is sprinting

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        // Set a default facing direction at the start of the game (e.g., facing down)
        lastDirection = new Vector3(0, -1, 0);
    }

    void Update()
    {

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            direction = Vector3.zero; // Set pergerakan ke nol
            animator.SetFloat("Speed", 0); // Paksa animasi idle
            return; // Lewati sisa fungsi Update
        }

        // --- Input ---
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, vertical, 0);

        // Check if the sprint key (Left Shift) is held down
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // --- Update Animator ---
        if (animator != null)
        {
            if (direction.magnitude > 0) // We are moving
            {
                lastDirection = direction.normalized; // Store direction for idle

                // Set CURRENT movement for the Run Blend Tree
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);
            }

            // Set the Speed for transitions
            animator.SetFloat("Speed", direction.sqrMagnitude);

            // Always update the LAST direction for the Idle Blend Tree
            animator.SetFloat("lastMoveX", lastDirection.x);
            animator.SetFloat("lastMoveY", lastDirection.y);
        }
    }

    private void FixedUpdate()
    {
        // Adjust speed based on whether the player is sprinting
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            direction = Vector3.zero; // Set pergerakan ke nol
            animator.SetFloat("Speed", 0); // Paksa animasi idle
            return; // Lewati sisa fungsi Update
        }
        
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        // Move the player
        transform.position += direction.normalized * currentSpeed * Time.deltaTime;
    }
}