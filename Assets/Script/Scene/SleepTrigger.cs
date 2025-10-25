using UnityEngine;

public class SleepTrigger : MonoBehaviour
{
    private bool playerIsNear = false;
    
    // Kita tidak perlu menyimpan referensi TimeManager di Awake()
    // private TimeManager timeManager; 

    // Kita tidak perlu fungsi Awake() sama sekali
    // private void Awake()
    // {
    //     timeManager = GameManager.Instance.timeManager;
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            // Panggil TimeManager Singleton SECARA LANGSUNG
            // Ini jauh lebih aman karena Instance pasti sudah di-set
            // pada saat Update() pertama kali berjalan.
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SleepAndAdvanceDay();
            }
            else
            {
                Debug.LogError("TimeManager.Instance tidak ditemukan!");
            }
        }
    }
}