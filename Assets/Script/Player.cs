using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public InventoryManager inventory;
    public TileManager tileManager;

    [Header("Interaction")]
    public float interactionRange = 2f; // Range untuk IInteractable
    public float tileInteractionDistance = 1.0f; // Jarak untuk cangkul

    [Header("Indicator")]
    [Tooltip("Seret GameObject 'TileIndicator' Anda ke sini")]
    public GameObject tileIndicator; 

    private IInteractable currentInteractable; 
    private Vector2 lastDirection = Vector2.down; 
    private Vector3Int lastTargetGridPos; // Menyimpan posisi grid terakhir

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            tileManager = GameManager.Instance.tileManager;
        }
        else
        {
            Debug.LogError("GameManager.Instance not found!");
        }

        if (tileIndicator != null)
        {
            tileIndicator.SetActive(false); // Pastikan indikator mati saat mulai
        }
    }

    private void Update()
    {
        // 1. Tentukan arah hadap pemain
        HandleDirection();
        
        // 2. Perbarui posisi dan visibilitas indikator ubin
        UpdateTileIndicator();

        // 3. Cek input untuk membajak (tombol E)
        HandlePlowInput();

        // 4. Cek input untuk interaksi objek (tombol F)
        HandleInteractInput();
    }

    private void HandleDirection()
    {
        if (animator.GetFloat("Speed") > 0.01f) 
        {
            lastDirection = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            if (lastDirection.sqrMagnitude > 0) lastDirection.Normalize();
        }
        else 
        {
            lastDirection = new Vector2(animator.GetFloat("lastMoveX"), animator.GetFloat("lastMoveY"));
            if (lastDirection.sqrMagnitude > 0) lastDirection.Normalize();
        }
    }

    private void UpdateTileIndicator()
    {
        if (tileManager == null || tileIndicator == null || inventory == null) return;

        // Tentukan ubin target
        Vector3 targetWorldPos = transform.position + (Vector3)lastDirection * tileInteractionDistance;
        Vector3Int targetGridPos = tileManager.WorldToCell(targetWorldPos);
        
        // Simpan posisi ini untuk digunakan oleh HandlePlowInput()
        lastTargetGridPos = targetGridPos; 

        // Tentukan posisi tengah ubin
        Vector3 cellCenterPos = tileManager.GetCellCenter(targetGridPos);
        tileIndicator.transform.position = cellCenterPos;

        // Cek apakah kita bisa berinteraksi dengan ubin ini
        string currentItem = inventory.toolbar.selectedSlot.itemName;
        string targetTileName = tileManager.GetTileName(targetGridPos);

        bool canPlow = (targetTileName == "Interactable_" && currentItem == "Hoe");
        bool canWater = (targetTileName == "Sowed tanah iyeah (updated0.1)" && currentItem == "WateringCan");

        // Tampilkan atau sembunyikan indikator
        if (canPlow || canWater)
        {
            tileIndicator.SetActive(true);
        }
        else
        {
            tileIndicator.SetActive(false);
        }
    }

    private void HandlePlowInput()
    {
        // Cek 'E' DAN apakah indikator sedang aktif (berarti valid)
        if (Input.GetKeyDown(KeyCode.E) && tileIndicator.activeSelf)
        {
            string currentItem = inventory.toolbar.selectedSlot.itemName;
            
            // Cek sekali lagi untuk alat yang benar (jika indikator dipakai >1 alat)
            if (currentItem == "Hoe")
            {
                animator.SetTrigger("IsPlowing");
                // Kita gunakan 'lastTargetGridPos' yang sudah dihitung
                tileManager.evolveTile(lastTargetGridPos); 
            }
            else if (currentItem == "WateringCan")
            {
                animator.SetTrigger("IsWatering");
                tileManager.WaterTile(lastTargetGridPos);
            }
        }
    }

    private void HandleInteractInput()
    {
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
            if (closest == null) return;

            closest.GetComponent<IInteractable>().Interact(); 
        }
    }

    // --- (Fungsi dropItem Anda tidak perlu diubah) ---
    public void dropItem(Item item)
    {
        Vector2 spawnLocation = transform.position;
        Vector2 spawnOffset = Random.insideUnitCircle * 1f;
        Item droppedItem = Instantiate(item, spawnLocation + spawnOffset, Quaternion.identity);
    }

    public void dropItem(Item item, int numToDrop)
    {
        for (int i = 0; i < numToDrop; i++)
        {
            dropItem(item);
        }
    }
}