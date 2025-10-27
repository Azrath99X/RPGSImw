// File: ExplorationTrigger.cs
using UnityEngine;

public class ExplorationTrigger : MonoBehaviour, IInteractable
{
    [Tooltip("Seret Panel UI (misal: StorageRoom_Panel) ke sini")]
    public CanvasGroup explorationPanel;
    
    [Tooltip("Seret Hotspot pertama (misal: Hotspot_Exit) ke sini")]
    public GameObject firstSelectable;

    [Header("Exit Behavior")]
    [Tooltip("Objek kosong di scene tempat Player akan dipindahkan setelah keluar")]
    public Transform exitTeleportTarget;

    public void Interact()
    {
        // Panggil manajer untuk masuk mode eksplorasi
        ExplorationModeManager.Instance.EnterExplorationMode(explorationPanel, firstSelectable, this);
    }
}