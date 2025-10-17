using UnityEngine;

// Ganti nama file ini kembali ke InteractiveTeleporter2D.cs atau nama aslinya
public class InteractiveTeleporter2D : MonoBehaviour 
{
    [Header("Tujuan Teleportasi")]
    public Transform teleportTarget;

    [Header("Manajemen Area")]
    public GameObject areaToEnable;
    public GameObject areaToDisable;
    
    private bool playerIsNear = false;
    private GameObject playerToTeleport; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            playerToTeleport = other.gameObject; 
            // Opsional: Tampilkan UI "Tekan [E]"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            playerToTeleport = null; 
            // Opsional: Sembunyikan UI "Tekan [E]"
        }
    }

    private void Update()
    {
        // Cek jika pemain ada di dekat, menekan E, dan tidak sedang transisi
        if (playerIsNear && Input.GetKeyDown(KeyCode.E) && playerToTeleport != null)
        {
            // Pastikan TransitionManager ada dan tidak sedang sibuk
            if(TransitionManager.Instance != null && !TransitionManager.Instance.isTransitioning)
            {
                // Panggil TransitionManager untuk melakukan pekerjaan
                TransitionManager.Instance.StartTeleportTransition(
                    playerToTeleport, 
                    teleportTarget, 
                    areaToEnable, 
                    areaToDisable
                );
            }
        }
    }
}