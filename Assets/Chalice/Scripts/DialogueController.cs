using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Akana {

public class DialogueController : MonoBehaviour {

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private UnityEvent skipHandler;


    // Update is called once per frame
    void Update() {
        skipHandler?.Invoke();
    }
}


} // namespace Akana