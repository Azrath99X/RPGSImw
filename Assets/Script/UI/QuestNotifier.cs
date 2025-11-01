// File: Script/UI/QuestNotifier.cs
using System.Collections;
using UnityEngine;
using TMPro;

public class QuestNotifier : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel pop-up yang akan muncul")]
    public GameObject notificationPanel;
    
    [Tooltip("Teks untuk 'Quest Baru: ...'")]
    public TextMeshProUGUI titleText;
    
    [Tooltip("Teks untuk nama quest-nya")]
    public TextMeshProUGUI questNameText;

    [Header("Settings")]
    [Tooltip("Berapa lama notifikasi muncul di layar")]
    public float displayDuration = 4.0f;

    // Referensi ke Animator panel
    private Animator panelAnimator;
    // Referensi ke coroutine yang sedang berjalan
    private Coroutine notificationCoroutine;

    void Awake()
    {
        // Ambil Animator dari panel. Kita akan menggunakannya untuk animasi slide-in/out
        panelAnimator = notificationPanel.GetComponent<Animator>();
        if (panelAnimator == null)
        {
            Debug.LogError("QuestNotifier: Tidak ada 'Animator' di notificationPanel!");
        }
        
        // Sembunyikan panel saat mulai
        notificationPanel.SetActive(false);
    }

    // --- LANGKAH PENTING (Subscribe/Unsubscribe) ---
    void OnEnable()
    {
        // Mulai "mendengarkan" event dari QuestManager
        QuestManager.OnQuestStarted += ShowNotification;
    }

    void OnDisable()
    {
        // Berhenti "mendengarkan" saat script dimatikan
        QuestManager.OnQuestStarted -= ShowNotification;
    }
    // ------------------------------------------------

    // Fungsi ini dipanggil secara otomatis oleh event OnQuestStarted
    private void ShowNotification(Quest quest)
    {
        // Jika notifikasi sebelumnya masih berjalan, hentikan
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        // Mulai coroutine baru untuk menampilkan notifikasi
        notificationCoroutine = StartCoroutine(ShowNotificationCoroutine(quest));
    }

    private IEnumerator ShowNotificationCoroutine(Quest quest)
    {
        // 1. Tampilkan panel dan atur teksnya
        notificationPanel.SetActive(true);
        titleText.text = "Quest Baru Diterima"; // Teks statis
        questNameText.text = quest.questName; // Nama quest dari data

        // 2. Putar animasi "Show" (yang akan kita buat di Animator)
        panelAnimator.Play("Show");

        // 3. Tunggu beberapa detik
        yield return new WaitForSeconds(displayDuration);

        // 4. Putar animasi "Hide"
        panelAnimator.Play("Hide");

        // 5. Tunggu animasi "Hide" selesai (animasi harus berdurasi 0.5 detik)
        yield return new WaitForSeconds(0.5f); 

        // 6. Matikan panel
        notificationPanel.SetActive(false);
        notificationCoroutine = null;
    }
}