// File: StoryManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Untuk pindah scene

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    // Ini adalah variabel untuk melacak trust
    public int trustAir = 0;
    public int trustXylem = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    // Fungsi ini yang akan kita panggil dari UnityEvent
    // Kita gunakan string agar fleksibel
    public void AddTrust(string characterName, int amount)
    {
        if (characterName == "Air")
        {
            trustAir += amount;
            Debug.Log("Trust Air: " + trustAir);
        }
        else if (characterName == "Xylem")
        {
            trustXylem += amount;
            Debug.Log("Trust Xylem: " + trustXylem);
        }
    }

    // Fungsi ini dipanggil di akhir game
    public void TriggerEnding()
    {
        Debug.Log("Memicu Ending... Air: " + trustAir + " | Xylem: " + trustXylem);
        
        if (trustAir > trustXylem && trustAir > 5)
        {
            SceneManager.LoadScene("Ending_Air"); // Ganti nama scene
        }
        else if (trustXylem > trustAir && trustXylem > 5)
        {
            SceneManager.LoadScene("Ending_Xylem"); // Ganti nama scene
        }
        else
        {
            SceneManager.LoadScene("Ending_Neutral"); // Ganti nama scene
        }
    }
}