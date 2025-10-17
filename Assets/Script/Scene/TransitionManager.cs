using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    // 1. Singleton Pattern
    public static TransitionManager Instance { get; private set; }

    [Header("Fade Effect")]
    [Tooltip("Panel UI yang memiliki CanvasGroup untuk fading.")]
    public CanvasGroup fadePanelCanvasGroup; 
    [Tooltip("Durasi fade-in dan fade-out.")]
    public float fadeDuration = 0.5f;

    // 2. Status Transisi
    public bool isTransitioning { get; private set; } = false;

    void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // 3. Fungsi publik yang dipanggil oleh teleporter
    public void StartTeleportTransition(GameObject player, Transform target, GameObject areaToEnable, GameObject areaToDisable)
    {
        // Hanya mulai jika tidak sedang transisi
        if (!isTransitioning)
        {
            StartCoroutine(FadeAndTeleport(player, target, areaToEnable, areaToDisable));
        }
    }
    
    // 4. Coroutine yang menjalankan seluruh proses
    private IEnumerator FadeAndTeleport(GameObject player, Transform target, GameObject areaToEnable, GameObject areaToDisable)
    {
        isTransitioning = true; // Set status sedang teleportasi

        // --- PHASE 1: Fade Out (layar jadi gelap) ---
        yield return StartCoroutine(FadeCanvasGroup(fadeDuration, 1f));

        // --- PHASE 2: Lakukan Teleportasi ---
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false; 
            player.transform.position = target.position;
            rb.simulated = true; 
        }
        else
        {
            player.transform.position = target.position;
        }
        player.transform.rotation = target.rotation;

        if (areaToEnable != null) areaToEnable.SetActive(true);
        if (areaToDisable != null) areaToDisable.SetActive(false); // Ini sekarang aman!

        Debug.Log($"Teleportasi 2D ke {areaToEnable.name}");

        // --- PHASE 3: Fade In (layar jadi terang kembali) ---
        yield return StartCoroutine(FadeCanvasGroup(fadeDuration, 0f));

        isTransitioning = false; // Teleportasi selesai
    }

    // 5. Fungsi Coroutine reusable untuk melakukan fade
    private IEnumerator FadeCanvasGroup(float duration, float targetAlpha)
    {
        if (fadePanelCanvasGroup == null)
        {
            Debug.LogWarning("FadePanel CanvasGroup tidak di-set di TransitionManager!");
            yield break; 
        }

        fadePanelCanvasGroup.blocksRaycasts = true;
        
        float startAlpha = fadePanelCanvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadePanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null; 
        }

        fadePanelCanvasGroup.alpha = targetAlpha; 

        if (targetAlpha == 0f) 
        {
            fadePanelCanvasGroup.blocksRaycasts = false;
        }
    }
}