using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Action : MonoBehaviour {
    public void skip(DialogueUI dialogueUI) {
        if (Input.GetMouseButtonUp(0)) {
            dialogueUI.MarkLineComplete();
        }
    }
}
