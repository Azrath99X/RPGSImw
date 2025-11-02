using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingScreen : MonoBehaviour
{
    [Tooltip("Waktu (detik) sebelum otomatis kembali ke Main Menu")]
    public float delayBeforeMainMenu = 10.0f; // Beri waktu 10 detik

    [Tooltip("Nama scene Main Menu Anda")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        StartCoroutine(GoToMainMenuAfterDelay());
    }

    private IEnumerator GoToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMainMenu);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Izinkan pemain skip dengan menekan tombol apapun
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}