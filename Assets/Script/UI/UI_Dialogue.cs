using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] private Image speakerPotrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI dialogueChoices;

    public void PlayDialogueLine(DialogueLine line)
    {
        speakerPotrait.sprite = line.speaker.speakerIcon;
        speakerName.text = line.speaker.speakerName;
        dialogueText.text = line.GetRandomLine();
    }
}
