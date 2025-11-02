using UnityEngine;
using System; // Diperlukan untuk 'Action'
using System.Collections; // Diperlukan untuk Coroutines
using UnityEngine.SceneManagement; //

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

    [Header("NPC Management")]
    public GameObject npc_Desta;
    public GameObject npc_Air;
    public GameObject npc_Xylem;

    public GameObject d2_ExitHouseTrigger;

    [Header("Dialogue Assets")]
    public DialogueConversation dreamDay1_Dialogue;
    public DialogueConversation normalSleepDay1_Dialogue;
    public DialogueConversation secretSleepDay2_Dialogue;
    public DialogueConversation uneasySleepDay2_Dialogue;

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

    public void SetCharacterActive(string characterName, bool isActive)
    {
        if (string.IsNullOrEmpty(characterName)) return;

        // Kita gunakan switch case agar rapi
        switch (characterName)
        {
            case "Desta":
                if (npc_Desta != null) npc_Desta.SetActive(isActive);
                break;
            case "Air":
                if (npc_Air != null) npc_Air.SetActive(isActive);
                break;
            case "Xylem":
                if (npc_Xylem != null) npc_Xylem.SetActive(isActive);
                break;
            default:
                Debug.LogWarning($"Nama karakter '{characterName}' tidak dikenal oleh TimeManager.");
                break;
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

        // 1. Fade Out
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(1f, 0.5f));
        }

        // --- LAYAR GELAP ---

        // 2. Logika Dialog
        if (CurrentDay == 1)
        {
            // Ini memutar dialog mimpi Anda
            if (QuestManager.Instance.GetQuestStatus("TalkedDestaD1") == QuestStatus.Active)
            {
                DialogueManager.Instance.StartConversation(dreamDay1_Dialogue);
            }
            else
            {
                DialogueManager.Instance.StartConversation(normalSleepDay1_Dialogue);
            }
            
            // Menunggu dialog selesai
            yield return new WaitUntil(() => DialogueManager.Instance.IsDialogueActive == false);

            // --- INI PERBAIKANNYA ---
            // Ganti komentar Anda dengan kode ini:
            QuestManager.Instance.CompleteQuestByID("Day1_Complete");
            // --------------------------
        }
        else if (CurrentDay == 2)
        {
            // Logika Hari 2 Anda sudah benar
            if (QuestManager.Instance.GetQuestStatus("SecretBlankBox") == QuestStatus.Active)
            {
                DialogueManager.Instance.StartConversation(secretSleepDay2_Dialogue);
            }
            else
            {
                DialogueManager.Instance.StartConversation(uneasySleepDay2_Dialogue);
            }
            yield return new WaitUntil(() => DialogueManager.Instance.IsDialogueActive == false);

            // Logika Ending Anda sudah benar
            Debug.Log("Hari 2 selesai. Memicu ending...");
            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.TriggerEnding();
            }
            else
            {
                Debug.LogError("StoryManager.Instance tidak ditemukan! Memuat Main Menu...");
                SceneManager.LoadScene("MainMenu");
            }
            isSleeping = false;
            yield break;
        }

        // --- DIALOG SELESAI, LAYAR MASIH GELAP ---

        // 4. GANTI HARI 
        CurrentDay++;
        Debug.Log($"Selamat datang di Hari {CurrentDay}!");

        // 5. TERBITKAN EVENT BIASA
        OnDayChanged?.Invoke();

        // 6. LOGIKA EVENT BARU
        CheckForDailyEvents();

        // 7. Beri jeda sedikit
        yield return new WaitForSeconds(1.0f);

        // 8. Fade In
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(0f, 0.5f));
        }

        Debug.Log("Pemain bangun.");
        isSleeping = false; 
    }
    
    public void ActivateCharacterWithFade(string characterName)
    {
        // Jangan jalankan jika sudah ada fade lain (opsional tapi aman)
        if (isSleeping) return; 

        StartCoroutine(FadeAndActivateCharacter(characterName));
    }

    // 2. COROUTINE (Yang melakukan pekerjaan)
    private IEnumerator FadeAndActivateCharacter(string characterName)
    {
        // Hentikan pemain agar tidak bergerak selama fade
        // Kita asumsikan Player punya skrip Movement
        Movement playerMovement = GameManager.Instance.player.GetComponent<Movement>();
        if (playerMovement != null) playerMovement.enabled = false;

        // 1. Fade Out (gelap)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(1f, 0.5f));
        }

        // --- LAYAR GELAP ---
        // 2. Aktifkan karakter (gunakan fungsi lama Anda)
        SetCharacterActive(characterName, true);
        // -------------------

        // 3. Fade In (terang)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(0f, 0.5f));
        }

        // 4. Kembalikan gerakan pemain
        if (playerMovement != null) playerMovement.enabled = true;
    }
    
    public void DeactivateCharacterWithFade(string characterName)
    {
        if (isSleeping) return; // Mencegah bentrok dengan tidur
        StartCoroutine(FadeAndDeactivateCharacter(characterName));
    }

    // 2. COROUTINE BARU
    //    Yang menjalankan proses fade
    private IEnumerator FadeAndDeactivateCharacter(string characterName)
    {
        // Hentikan gerakan pemain
        Movement playerMovement = null;
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerMovement = GameManager.Instance.player.GetComponent<Movement>();
        }
        
        if (playerMovement != null) playerMovement.enabled = false;
        
        // 1. Fade Out (gelap)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(1f, 0.5f));
        }

        // --- LAYAR GELAP ---
        // 2. Nonaktifkan karakter (gunakan fungsi lama Anda)
        SetCharacterActive(characterName, false);
        // -------------------

        // 3. Fade In (terang)
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(0f, 0.5f));
        }

        // 4. Kembalikan gerakan pemain
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // 10. FUNGSI BARU UNTUK CEK EVENT
    private void CheckForDailyEvents()
    {
        // Cek apakah hari ini adalah hari pedagang tiba
        if (CurrentDay == merchantArrivalDay)
        {
            // Belum day 4
        }


        else if (CurrentDay == 2)
        {
            Debug.Log("Selamat Datang di Hari 2. Menukar Desta dengan Air.");
            
            // Nonaktifkan Desta
            if (npc_Desta != null)
                npc_Desta.SetActive(false);
            
            // Aktifkan Air
            if (npc_Air != null)
                npc_Air.SetActive(true);

            // Pastikan Xylem (yang akan muncul nanti) non-aktif
            if (npc_Xylem != null)
                npc_Xylem.SetActive(false);
                
            if (d2_ExitHouseTrigger != null)
            {
                d2_ExitHouseTrigger.SetActive(true);
            }
        }
    }
    
    public void LoadDay(int day)
    {
        CurrentDay = day;
        Debug.Log($"[SaveData] Berhasil load ke Hari {CurrentDay}");
    }
}