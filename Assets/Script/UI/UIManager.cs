using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // CORRECTED: Replaced 'InventoryUI' with 'Inventory_Ui'
    public Dictionary<string, Inventory_Ui> inventoryUIByName = new Dictionary<string, Inventory_Ui>();
    public GameObject inventoryPanel;
    public List<Inventory_Ui> inventoryUIs;

    

    public static Slot_Ui draggedSlot;
    public static Image draggedImage;
    public static bool dragAll; 

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel; // Seret Panel UI Anda ke sini
    private bool isPaused = false;    
    
    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        inventoryPanel.SetActive(false);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toogleInventory();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            dragAll = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            dragAll = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(isPaused);
        }
    }
    
    public void toogleInventory()
    {
        if(inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            inventoryPanel.SetActive(true);
            RefreshInventoryUI("Backpack");
        }
    }

    public void RefreshInventoryUI(string name)
    {
        if (inventoryUIByName.ContainsKey(name))
        {
            inventoryUIByName[name].Refresh();
        }
    }

    public void RefreshAll()
    {
        // CORRECTED: Replaced 'InventoryUI' with 'Inventory_Ui'
        foreach (KeyValuePair<string, Inventory_Ui> KeyValuePair in inventoryUIByName)
        {
            KeyValuePair.Value.Refresh();
        }
    }

    void Initialize()
    {
        foreach (Inventory_Ui inventoryUI in inventoryUIs)
        {
            // Note: This line uses 'inventoryUI.name'. This gets the GameObject's name.
            // Ensure your GameObjects have unique and correct names.
            if(!inventoryUIByName.ContainsKey(inventoryUI.inventoryName))
            {
                // CORRECTED: The second argument must also match the Dictionary definition
                inventoryUIByName.Add(inventoryUI.inventoryName, inventoryUI);
            }
        }
    }

    public Inventory_Ui GetInventoryUI(string name)
    {
        if (inventoryUIByName.ContainsKey(name))
        {
            return inventoryUIByName[name];
        }
        Debug.LogError("Inventory UI not found: " + name);
        return null;
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // Hentikan waktu di dalam game (ini akan menghentikan 'FixedUpdate' di Movement.cs)
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Lanjutkan waktu di dalam game
        Time.timeScale = 1f;
    }
    
    public void GoToMainMenu()
    {
        // PENTING: Selalu kembalikan Time.timeScale ke 1 sebelum pindah scene
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); // Ganti "MainMenu" dengan nama scene Main Menu Anda
    }
}