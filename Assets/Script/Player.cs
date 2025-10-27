using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public InventoryManager inventory;
    public TileManager tileManager;

    [Header("Interaction")]
    public float interactionRange = 2f; 
    public float tileInteractionDistance = 1.0f; 

    [Header("Indicator")]
    [Tooltip("Seret GameObject 'TileIndicator' Anda ke sini")]
    public GameObject tileIndicator; 

    private IInteractable currentInteractable; 
    private Vector2 lastDirection = Vector2.down; 
    private Vector3Int lastTargetGridPos; 

    // Ganti nama-nama ini agar SAMA PERSIS dengan nama item di Inventory Anda
    private const string HOE_ITEM_NAME = "Hoe";
    private const string WATERING_CAN_ITEM_NAME = "WateringCan";
    private const string SEED_ITEM_NAME = "TurnipSeed"; // Ganti jika nama benih Anda beda


    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.tileManager != null)
            tileManager = GameManager.Instance.tileManager;
        else
            tileManager = TileManager.Instance; 

        if (tileManager == null) Debug.LogError("TileManager tidak ditemukan!");
        if (tileIndicator != null) tileIndicator.SetActive(false);
    }

    private void Update()
    {
        if (tileManager == null || inventory == null || animator == null) return;

        HandleDirection();
        UpdateTileIndicator();
        HandleToolInput();
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
        if (tileIndicator == null) return;

        Vector3 targetWorldPos = transform.position + (Vector3)lastDirection * tileInteractionDistance;
        lastTargetGridPos = tileManager.WorldToCell(targetWorldPos);
        tileIndicator.transform.position = tileManager.GetCellCenter(lastTargetGridPos);

        // --- Logika Menampilkan Indikator ---
        string currentItem = inventory.toolbar.selectedSlot.itemName;
        string targetTileName = tileManager.GetTileName(lastTargetGridPos);
        
        bool canPlow = (currentItem == HOE_ITEM_NAME && targetTileName == tileManager.GetHiddenTileName());
        
        // DIPERBARUI: Sekarang cek semua tahap kering
        bool canWater = (currentItem == WATERING_CAN_ITEM_NAME && 
                        (targetTileName == tileManager.GetPlowedTileName() || 
                         targetTileName == tileManager.GetSeededTileName() || 
                         targetTileName == tileManager.GetSproutTileName() ||
                         targetTileName == tileManager.GetGrowthDay2DryName() || // BARU
                         targetTileName == tileManager.GetGrowthDay3DryName() )); // BARU
        
        bool canPlant = (currentItem == SEED_ITEM_NAME && 
                        (targetTileName == tileManager.GetPlowedTileName() || 
                         targetTileName == tileManager.GetWetPlowedTileName()));

        if (canPlow || canWater || canPlant)
            tileIndicator.SetActive(true);
        else
            tileIndicator.SetActive(false);
    }

    private void HandleToolInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && tileIndicator.activeSelf)
        {
            string currentItem = inventory.toolbar.selectedSlot.itemName;
            
            if (currentItem == HOE_ITEM_NAME)
            {
                animator.SetTrigger("IsPlowing");
                tileManager.evolveTile(lastTargetGridPos); 
            }
            else if (currentItem == WATERING_CAN_ITEM_NAME) 
            {
                tileManager.WaterTile(lastTargetGridPos);
            }
            else if (currentItem == SEED_ITEM_NAME)
            {
                bool success = tileManager.PlantSeed(lastTargetGridPos);
                if (success)
                {
                    // TODO: Kurangi benih dari inventory
                }
            }
        }
    }

    private void HandleInteractInput()
    {
        if (ExplorationModeManager.Instance != null && ExplorationModeManager.Instance.IsExploring)
    {
        return;
    }
        // Interaksi Tombol F untuk objek IInteractable
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Cek dulu apakah dialog sedang berjalan
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                // Jika ya, lanjutkan dialognya
                DialogueManager.Instance.AdvanceConversation();
                return; // Hentikan fungsi di sini
            }

            // Jika dialog tidak berjalan, baru cari interaksi baru
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
    // --- Fungsi Drop Item ---
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