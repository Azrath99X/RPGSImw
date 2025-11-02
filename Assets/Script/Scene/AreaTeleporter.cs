using UnityEngine;
// using UnityEngine.SceneManagement; // Tidak diperlukan
using System.Collections; // Diperlukan untuk Coroutine

public class AreaTeleporter : MonoBehaviour
{
    [Tooltip("Target GameObject kosong tempat pemain akan muncul")]
    public Transform destinationTransform;
    
    // Melacak apakah pemain ada di dalam trigger
    private bool playerIsNear = false;
    private bool isTeleporting = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika yang masuk adalah Player
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cek jika yang keluar adalah Player
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    private void Update()
    {
        // Cek jika pemain ada di dalam, menekan 'E', dan kita tidak sedang teleport
        if (playerIsNear && Input.GetKeyDown(KeyCode.E) && !isTeleporting)
        {
            if (destinationTransform == null)
            {
                Debug.LogWarning("Teleporter ini tidak punya destinasi!", this.gameObject);
                return;
            }
            
            // Mulai proses teleport
            StartCoroutine(TeleportSequence());
        }
    }

    private IEnumerator TeleportSequence()
    {
        isTeleporting = true;
        
        // 1. Hentikan Gerakan Pemain
        Movement playerMovement = null;
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerMovement = GameManager.Instance.player.GetComponent<Movement>();
        }
        
        if (playerMovement != null) 
            playerMovement.enabled = false;

        // 2. Panggil Fade Out
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(1f, 0.5f)); 
        }

        // --- LAYAR GELAP ---

        // 3. Pindahkan Posisi Player
        GameManager.Instance.player.transform.position = destinationTransform.position;
        
        // Beri jeda 1 frame agar posisi ter-update sebelum fade in
        yield return null; 

        // 4. Panggil Fade In
        if (TransitionManager.Instance != null)
        {
            yield return StartCoroutine(TransitionManager.Instance.Fade(0f, 0.5f)); 
        }
        
        // 5. Kembalikan gerakan pemain
        if (playerMovement != null) 
            playerMovement.enabled = true;

        // 6. Selesai
        isTeleporting = false;
    }
}