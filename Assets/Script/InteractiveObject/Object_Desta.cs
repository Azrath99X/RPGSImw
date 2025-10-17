using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Desta : Object_NPC, IInteractable
{

    [Header("Dialogue")]
    [SerializeField] private DialogueLine firstDialogueLine;

    protected override void Awake()
    {
        base.Awake();
        //switch to desta
    }

    protected override void Update()
    {
        base.Update();
    }

    public void Interact()
    {
        ui.OpenDialogueUI(firstDialogueLine);
    }
}
