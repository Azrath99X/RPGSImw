using UnityEngine;
using System; // Diperlukan untuk 'Action'
using System.Collections; // Diperlukan untuk Coroutines

public class TimeManager : MonoBehaviour
{
    // 1. Singleton Pattern (agar mudah diakses)
    public static TimeManager Instance { get; private set; }

    // 2. Event (Pengumuman)
    // Event umum untuk pergantian hari (untuk TileManager)
    public static event Action OnDayChanged; 
    
    // BARU: Event spesifik untuk pedagang
    public static event Action OnMerchantArrive;

    // 3. State (Status)
    // Counter hari Anda sudah ada di sini!
    public int CurrentDay { get; private set; } = 1;
    
    [Header("Event Settings")]
    [Tooltip("Di hari ke berapa pedagang akan tiba")]
    public int merchantArrivalDay = 4; // Anda bisa ubah ini di Inspector

    // Mencegah spam tombol tidur
    private bool isSleeping = false; 

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

    // 4. Fungsi Publik yang akan dipanggil oleh 'SleepTrigger'
    public void SleepAndAdvanceDay()
    {
        if (isSleeping) return;
        StartCoroutine(SleepSequence());
    }

    private IEnumerator SleepSequence()
    {
        isSleeping = true;
        
        Debug.Log("Pemain mulai tidur...");

        // 5. Fade Out (gelap)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(1f, 0.5f));
        }

        // --- INTI LOGIKA ---
        // 6. GANTI HARI (Counter bertambah di sini)
        CurrentDay++;
        Debug.Log($"Selamat datang di Hari {CurrentDay}!");

        // 7. TERBITKAN EVENT BIASA (untuk tanaman tumbuh)
        OnDayChanged?.Invoke();
        
        // 8. LOGIKA EVENT BARU: Cek event harian
        CheckForDailyEvents();
        // --------------------

        // Beri jeda sedikit
        yield return new WaitForSeconds(1.0f); 

        // 9. Fade In (terang)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(0f, 0.5f));
        }

        Debug.Log("Pemain bangun.");
        isSleeping = false; 
    }
    
    // 10. FUNGSI BARU UNTUK CEK EVENT
    private void CheckForDailyEvents()
    {
        // Cek apakah hari ini adalah hari pedagang tiba
        if (CurrentDay == merchantArrivalDay)
        {
        }
        
        // Anda bisa tambahkan 'else if' untuk event lain
        // else if (CurrentDay == 5) { // Festival Dimulai }
        // else if (CurrentDay % 7 == 0) { // Event mingguan }
    }
}