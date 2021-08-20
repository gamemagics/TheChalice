using UnityEngine;
using Yarn.Unity;

namespace Akana {

/// <summary>
/// Action handler for Skipping current line.
/// </summary>
public class SkipAction : MonoBehaviour {
    /// <summary>
    /// Skip current line.
    /// </summary>
    /// <param name="dialogueUI">Dialogue UI object</param>
    public void skip(DialogueUI dialogueUI) {
        if (Input.GetMouseButtonUp(0)) {
            dialogueUI.MarkLineComplete();
        }
    }
}

} // namespace Akana
