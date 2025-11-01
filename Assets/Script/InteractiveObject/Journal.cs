// File: Journal.cs
using UnityEngine;

public class Journal : MonoBehaviour, IInteractable
{
    // Kita akan butuh UI untuk konfirmasi, tapi untuk sekarang:
    // F = Save
    // E = Load (Hanya untuk testing!)
    
    // Fungsi ini dipanggil oleh Player.cs saat tombol 'F' ditekan
    public void Interact()
    {
        // Cek agar tidak membuka UI saat dialog lain aktif
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;
        
        // Panggil Singleton UI Jurnal untuk membuka panel
        Journal_Ui.Instance.OpenJournal();
    }
    
    // Kita bisa tambahkan cek input di sini untuk testing cepat
    private bool playerIsNear = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = false;
    }

    private void Update()
    {
        if (playerIsNear)
        {
            // Tombol F untuk Interact() sudah ditangani Player.cs
            // Jadi kita harus gunakan tombol lain untuk Save/Load
            
            if (Input.GetKeyDown(KeyCode.S)) // 'S' untuk Save
            {
                SaveLoadManager.Instance.SaveGame();
            }

            if (Input.GetKeyDown(KeyCode.L)) // 'L' untuk Load
            {
                SaveLoadManager.Instance.LoadGame();
            }
        }
    }
}