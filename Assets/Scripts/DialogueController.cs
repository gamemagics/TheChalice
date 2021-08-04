using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class DialogueController : MonoBehaviour {

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private UnityEvent skipHandler;
    private bool isOptionsDisplayed = false;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        skipHandler?.Invoke();
        // if (!isOptionsDisplayed) {
        //     skipHandler?.Invoke();
        // }
    }

    public void EnableOptions(bool flag) {
        isOptionsDisplayed = flag;
    }
}
