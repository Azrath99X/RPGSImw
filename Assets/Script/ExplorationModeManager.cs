// File: ExplorationModeManager.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ExplorationModeManager : MonoBehaviour
{
    public static ExplorationModeManager Instance { get; private set; }

    [Header("Player Components")]
    public Movement playerMovement;
    public SpriteRenderer playerSprite;     
    // Ubah ini dari 'private'
    private bool isExploring = false;
    // Buat properti publik agar skrip lain bisa membacanya
    public bool IsExploring { get { return isExploring; } }
    
    private CanvasGroup currentExplorationPanel;
    private GameObject lastSelectedObject;
    
    private ExplorationTrigger currentTrigger;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // --- TAMBAHKAN FUNGSI UPDATE INI ---
    void Update()
    {
        // Jika kita tidak sedang eksplorasi, jangan lakukan apa-apa
        if (!isExploring) return;

        // --- PERBAIKAN FOKUS EVENT SYSTEM ---
        if (EventSystem.current.currentSelectedGameObject == null && lastSelectedObject != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        }
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
        // --- AKHIR PERBAIKAN ---


        // --- 1. CEK UNTUK KELUAR (TOMBOL 'E') ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Jangan biarkan keluar jika dialog sedang berjalan
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                Debug.Log("Mencoba keluar dengan 'E', tetapi dialog sedang aktif.");
                return; 
            }

            Debug.Log("'E' ditekan, keluar dari mode eksplorasi.");
            ExitExplorationMode();
            return; // Hentikan di sini agar 'F' tidak ikut terproses
        }
        // --- AKHIR BLOK 'E' ---


        // --- 2. CEK UNTUK INTERAKSI (TOMBOL 'F') ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Cek apakah dialog sedang aktif
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                // JIKA AKTIF: Panggil AdvanceConversation()
                DialogueManager.Instance.AdvanceConversation();
            }
            else // JIKA TIDAK AKTIF: Mulai percakapan baru
            {
                GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
                if (selectedObject != null)
                {
                    Button selectedButton = selectedObject.GetComponent<Button>();
                    if (selectedButton != null && selectedButton.interactable)
                    {
                        // Panggil OnClick() untuk MEMULAI dialog
                        selectedButton.onClick.Invoke();
                    }
                }
            }
        }
    }// ------------------------------------

    public void EnterExplorationMode(CanvasGroup panel, GameObject firstSelectable, ExplorationTrigger trigger)
    {
        currentExplorationPanel = panel;
        currentTrigger = trigger; // <-- Simpan trigger yang memanggil kita

        // 1. Sembunyikan Player
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerSprite != null) playerSprite.enabled = false;
        
        // 2. Tampilkan Panel
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;

        // 3. Ambil alih input
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectable);
        lastSelectedObject = firstSelectable;
        
        isExploring = true;
    }

    // --- UBAH KEDUA FUNGSI INI ---
    // (Kembalikan ExitExplorationMode ke versi non-Coroutine)
    public void ExitExplorationMode()
    {
        Debug.Log("--- Mencoba Keluar dari Mode Eksplorasi ---");

        if (!isExploring) 
        {
            Debug.LogError("GAGAL KELUAR: 'isExploring' adalah false.");
            return; 
        }

        if (currentExplorationPanel == null) 
        {
            Debug.LogError("GAGAL KELUAR: 'currentExplorationPanel' adalah NULL.");
            return;
        }

        Debug.Log("BERHASIL: Panel ditemukan, sedang menutup panel...");

        // 1. Sembunyikan Panel
        currentExplorationPanel.alpha = 0;
        currentExplorationPanel.interactable = false;
        currentExplorationPanel.blocksRaycasts = false;

        // 2. Kembalikan input
        EventSystem.current.SetSelectedGameObject(null);
        currentExplorationPanel = null;
        isExploring = false; 

        // 3. KEMBALIKAN PLAYER (Teleportasi & Aktifkan)
        if (playerMovement != null)
        {
            // --- LOGIKA TELEPORTASI BARU ---
            if (currentTrigger != null && currentTrigger.exitTeleportTarget != null)
            {
                // Teleportasi pemain ke target
                playerMovement.transform.position = currentTrigger.exitTeleportTarget.position;
                Debug.Log("Player diteleportasi ke " + currentTrigger.exitTeleportTarget.name);
            }
            else
            {
                Debug.LogWarning("Tidak ada exitTeleportTarget! Player akan muncul di posisi lama.");
            }
            // --- AKHIR LOGIKA BARU ---

            // Aktifkan kembali movement
            playerMovement.enabled = true;
        }
        
        if (playerSprite != null) playerSprite.enabled = true;

        // Hapus referensi trigger
        currentTrigger = null;
    }
}
