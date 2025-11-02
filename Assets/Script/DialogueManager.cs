// File: DialogueManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive { get; private set; }
    public bool IsDisplayingChoices { get; private set; }

    [Header("UI Components")]
    public GameObject dialogueBox; // Panel utama UI dialog
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueLineText;
    public Transform choiceButtonContainer;
    public GameObject choiceButtonPrefab;
    public Image characterPortraitImage;

    private Queue<DialogueLine> currentLines;
    private DialogueConversation currentConversation;

    [Header("UI to Toggle")]
    [Tooltip("Seret GameObject UI Toolbar Anda ke sini")]
    public List<GameObject> uiElementsToHide; // <-- GUNAKAN INI



    

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            currentLines = new Queue<DialogueLine>();
        }
    }

    void Start()
    {
        dialogueBox.SetActive(false);
        IsDialogueActive = false; 
        IsDisplayingChoices = false;
    }
    
    // Panggil ini untuk memulai percakapan
    public void StartConversation(DialogueConversation conversation)
    {
        if (conversation == null)
        {
            EndConversation();
            return;
        }

        // Nonaktifkan kontrol pemain di sini
        // playerController.enabled = false; 

        dialogueBox.SetActive(true);
        IsDialogueActive = true; 
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false); 
        }
        currentConversation = conversation;
        currentLines.Clear();

        foreach (DialogueLine line in conversation.lines)
        {
            currentLines.Enqueue(line);
        }

        DisplayNextLine();
        
    }


    public void AdvanceConversation()
    {
        if (currentLines.Count > 0)
        {
            DisplayNextLine();
        }
        else
        {
            // Jika tidak ada baris lagi, tampilkan pilihan atau akhiri
            ShowChoicesOrEnd();
        }
    }

    private void DisplayNextLine()
    {
        if (currentLines.Count == 0)
        {
            ShowChoicesOrEnd();
            return;
        }

        DialogueLine line = currentLines.Dequeue();
        
        if (line.character != null)
        {
            // Tampilkan nama
            speakerNameText.text = line.character.characterName;

            // Tampilkan potret (LOGIKA BARU)
            if (line.character.portrait != null)
            {
                characterPortraitImage.sprite = line.character.portrait;
                characterPortraitImage.enabled = true; // Tampilkan gambar
            }
            else
            {
                // Sembunyikan jika karakter tidak punya potret
                characterPortraitImage.enabled = false; 
            }
        }
        else
        {
            // Ini untuk Narator
            speakerNameText.text = "Narrator";
            // Sembunyikan potret untuk Narator
            characterPortraitImage.enabled = false; 
        }

        // Hentikan coroutine typewriter jika ada
        StopAllCoroutines();
        StartCoroutine(TypewriterEffect(line.line));
    }

    // Efek ketik opsional namun bagus
    private IEnumerator TypewriterEffect(string line)
    {
        dialogueLineText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueLineText.text += letter;
            yield return new WaitForSeconds(0.02f); // Sesuaikan kecepatan ketik
        }
    }

    private void ShowChoicesOrEnd()
    {
        // Hapus tombol pilihan lama
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Cek jika ada pilihan 
        if (currentConversation.playerChoices.Length > 0)
        {
            IsDisplayingChoices = true;
            for (int i = 0; i < currentConversation.playerChoices.Length; i++)
            {
                GameObject buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                PlayerChoice choice = currentConversation.playerChoices[i];

                buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
                buttonGO.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choice));
            }
        }
        else
        {
            // Tidak ada pilihan, panggil event akhir

            // --- HAPUS INI ---
            // currentConversation.onConversationEnd?.Invoke();
            IsDisplayingChoices = false;
            
            // --- LOGIKA BARU DI SINI ---
            TriggerConversationEndEvents(currentConversation);
            // ---------------------------
            
            // Lanjutkan ke percakapan berikutnya (jika ada)
            if (currentConversation.nextConversationOnEnd != null)
            {
                StartConversation(currentConversation.nextConversationOnEnd);
            }
            else
            {
                EndConversation();
            }
        }
    }

    private void SelectChoice(PlayerChoice choice)
    {
        IsDisplayingChoices = false;
        // Hapus tombol pilihan
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // --- HAPUS INI ---
        // choice.onChoiceSelected?.Invoke();

        // --- LOGIKA BARU DI SINI ---
        // 1. Berikan Quest jika ada
        if (choice.questToStartOnSelect != null)
        {
            // Pastikan QuestManager Anda adalah Singleton
            QuestManager.Instance.StartQuest(choice.questToStartOnSelect);
        }
        
        // 2. Berikan Trust jika ada
        if (!string.IsNullOrEmpty(choice.characterToTrust))
        {
            // Pastikan StoryManager Anda adalah Singleton
            StoryManager.Instance.AddTrust(choice.characterToTrust, choice.trustAmount);
        }
        // ---------------------------

        // Mulai percakapan berikutnya berdasarkan pilihan
        if (choice.nextConversation != null)
        {
            StartConversation(choice.nextConversation);
        }
        else
        {
            // Jika pilihan ini mengakhiri dialog, panggil juga event akhir
            TriggerConversationEndEvents(currentConversation);
            EndConversation();
        }
    }

    // --- BUAT FUNGSI BARU INI ---
    private void TriggerConversationEndEvents(DialogueConversation conversation)
    {
        // 1. Berikan Quest jika ada
        if (conversation.questToStartOnEnd != null)
        {
            QuestManager.Instance.StartQuest(conversation.questToStartOnEnd);
        }

        // 2. Berikan Item jika ada
        if (conversation.itemToGiveOnEnd != null)
        {
            Inventory playerBackpack = GameManager.Instance.player.inventory.backpack;

            // Debug info to help identify inventory space issues
            int emptySlots = 0;
            int totalSlots = playerBackpack.slots.Count;
            foreach (var slot in playerBackpack.slots)
            {
                if (slot.isEmpty) emptySlots++;
            }

            Debug.Log($"Attempting to add {conversation.itemGiveAmount}x {conversation.itemToGiveOnEnd.itemName}. Backpack: {emptySlots}/{totalSlots} empty slots");

            // Check if there's enough space before adding
            if (playerBackpack.CanAdd(conversation.itemToGiveOnEnd, conversation.itemGiveAmount))
            {
                playerBackpack.Add(conversation.itemToGiveOnEnd, conversation.itemGiveAmount);
                Debug.Log($"Successfully added {conversation.itemGiveAmount}x {conversation.itemToGiveOnEnd.itemName} to inventory");
            }
            else
            {
                Debug.LogWarning($"Cannot add {conversation.itemGiveAmount}x {conversation.itemToGiveOnEnd.itemName} - insufficient inventory space!");
                // TODO: Show UI message to player about full inventory
            }


        }
        if (!string.IsNullOrEmpty(conversation.characterToActivateOnEnd))
        {
            // --- GANTI BLOK INI ---
            // TimeManager.Instance.SetCharacterActive(conversation.characterToActivateOnEnd, true);

            // --- MENJADI BLOK INI ---
            if (conversation.useFadeForActivation)
            {
                // Panggil fungsi FADE baru di TimeManager
                TimeManager.Instance.ActivateCharacterWithFade(conversation.characterToActivateOnEnd);
            }
            else
            {
                // Gunakan cara instan yang lama
                TimeManager.Instance.SetCharacterActive(conversation.characterToActivateOnEnd, true);
            }
            // ---------------------
        }
        // 4. Non-aktifkan Karakter
        if (!string.IsNullOrEmpty(conversation.characterToDeactivateOnEnd))
        {
            // --- GANTI BLOK INI ---
            // TimeManager.Instance.SetCharacterActive(conversation.characterToDeactivateOnEnd, false);

            // --- MENJADI BLOK INI ---
            if (conversation.useFadeForDeactivation)
            {
                // Panggil fungsi FADE baru di TimeManager
                TimeManager.Instance.DeactivateCharacterWithFade(conversation.characterToDeactivateOnEnd);
            }
            else
            {
                // Gunakan cara instan yang lama
                TimeManager.Instance.SetCharacterActive(conversation.characterToDeactivateOnEnd, false);

            }
        }
    }

    private void EndConversation()
    {
        dialogueBox.SetActive(false);
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(true); 
        }
        IsDialogueActive = false; 
        // Aktifkan kembali kontrol pemain di sini      
        // playerController.enabled = true; 
    }
}