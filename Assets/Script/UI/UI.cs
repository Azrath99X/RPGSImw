using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    [SerializeField] private GameObject uiElements;
    public bool alternativeInput { get; private set; }

    #region UI Components
    public Inventory_Ui inventoryUi { get; private set; }
    public Toolbar_UI toolbarUi { get; private set; }
    public UI_Dialogue dialogueUi { get; private set; }
    #endregion

    private void Awake()
    {
        instance = this;

        inventoryUi = GetComponentInChildren<Inventory_Ui>(true);
        toolbarUi = GetComponentInChildren<Toolbar_UI>(true);
        dialogueUi = GetComponentInChildren<UI_Dialogue>(true);
    }


    public void OpenDialogueUI(DialogueLine FirstLine)
    {
        dialogueUi.gameObject.SetActive(true);
        dialogueUi.PlayDialogueLine(FirstLine);
    }

    public void OpenInventoryUI()
    {
            inventoryUi.gameObject.SetActive(true);
        
    }
}