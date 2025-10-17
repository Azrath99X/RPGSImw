using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SimRpg/Dialogue Data/New Line Data", fileName = "Line -")]
public class DialogueLine : ScriptableObject
{
    [Header("Dialogue info")]
    public string dialogueGroupName;
    public DialogueSpeaker speaker;

    [Header("Text options")]
    [TextArea] public string[] textline;

    [Header("Answer setup")]
    public bool playCanAnswer;
    public DialogueLine[] answerLine;


    public string GetRandomLine()
    {
        return textline[Random.Range(0, textline.Length)];
    }
}
