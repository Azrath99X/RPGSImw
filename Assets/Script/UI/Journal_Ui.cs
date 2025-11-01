// File: Script/UI/Journal_Ui.cs
// Versi ini memperbaiki typo 'D' dan memastikan tidak ada fungsi duplikat.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class Journal_Ui : MonoBehaviour
{
    public static Journal_Ui Instance { get; private set; }

    [Header("UI Components")]
    public Button saveButton;
    public Button loadButton;
    public Button closeButton;
    
    [Tooltip("Teks untuk info (misal: 'Belum ada save file')")]
    public TextMeshProUGUI saveInfoText;

    private CanvasGroup canvasGroup;
    private Movement playerMovement; // Referensi untuk membekukan pemain

    void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        // Pastikan panel tersembunyi saat mulai
        CloseJournal();

        // Cari referensi player movement dengan aman
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerMovement = GameManager.Instance.player.GetComponent<Movement>();
        }

        // Hubungkan fungsi tombol
        saveButton.onClick.AddListener(OnSaveButton);
        loadButton.onClick.AddListener(OnLoadButton);
        closeButton.onClick.AddListener(OnCloseButton);
    }

    public void OpenJournal()
    {
        // Tampilkan panel
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Bekukan pemain
        if (playerMovement != null) playerMovement.enabled = false;

        // Refresh info file save
        RefreshInfo();
    }

    // Hanya ada SATU fungsi CloseJournal()
    public void CloseJournal()
    {
        // Sembunyikan panel
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Kembalikan kontrol pemain
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // Dipanggil saat panel dibuka
    private void RefreshInfo()
    {
        if (SaveLoadManager.Instance.HasSaveFile())
        {
            loadButton.interactable = true;
            saveInfoText.text = "File save ditemukan.";
        }
        else
        {
            loadButton.interactable = false;
            saveInfoText.text = "Belum ada file save.";
        }
    }

    // --- Fungsi Tombol ---
    private void OnSaveButton()
    {
        SaveLoadManager.Instance.SaveGame();
        RefreshInfo(); // Update info setelah save
    }

    private void OnLoadButton()
    {
        SaveLoadManager.Instance.LoadGame();
        CloseJournal(); // Tutup UI setelah load
    }

    // --- BAGIAN YANG DIPERBAIKI ---
    // (Tidak ada 'D' dan memiliki body '{ }' yang benar)
    private void OnCloseButton()
    {
        CloseJournal();
    }
}