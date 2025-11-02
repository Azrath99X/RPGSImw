using UnityEngine;
using UnityEngine.SceneManagement; // Penting untuk pindah scene

public class MainMenu : MonoBehaviour
{
    [Tooltip("Nama scene game utama Anda, misal 'SampleScene'")]
    public string gameSceneName = "SampleScene"; // Pastikan ini sama dengan nama scene game Anda

    // Fungsi ini akan dipasang di tombol "StartButton"
    public void StartGame()
    {// Memuat scene game Anda
        SceneManager.LoadScene(gameSceneName);
    }

    // Fungsi ini akan dipasang di tombol "QuitButton"
    public void QuitGame()
    {
        // Ini hanya berfungsi di build .exe, tidak di Unity Editor
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}