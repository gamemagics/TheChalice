using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEffects : MonoBehaviour {

    [SerializeField] private TMP_Text text = null;

    private delegate IEnumerator EffectFunction(TMP_Text textMesh);

    private EffectFunction[] functions = new EffectFunction[1];

    private Coroutine[] coroutines = new Coroutine[1];

    private static System.Random random = new System.Random();

    private static Vector2[] offset = new Vector2[1024];

    private void Awake() {
        functions[0] = new EffectFunction(Tremble);
    }

    public void OnDialogueUpdate() {
        for (int i = 0; i < functions.Length; ++i) {
            functions[i](text);
        }
    }

    public void OnDialogueWait() {
        for (int i = 0; i < functions.Length; ++i) {
            coroutines[i] = StartCoroutine(functions[i](text));
        }
    }

    public void OnDialogueContinue() {
        for (int i = 0; i < functions.Length; ++i) {
            if (coroutines[i] != null) {
                StopCoroutine(coroutines[i]);
                coroutines[i] = null;
            }
        }

        resume();
    }

    private void resume() {
        text.renderMode = TextRenderFlags.Render;
        text.ForceMeshUpdate();
    }

    private static IEnumerator Tremble(TMP_Text textMesh) {
        var info = textMesh.GetTextInfo(textMesh.text);
        while (true) {
            Vector3[] vertices = textMesh.mesh.vertices;
            textMesh.renderMode = TextRenderFlags.DontRender;
            textMesh.ForceMeshUpdate();

            for (int i = 0; i < info.linkCount; ++i) {
                var link = info.linkInfo[i];
                if (link.GetLinkID() != "tremble") {
                    continue;
                }

                for (int j = 0; j < link.linkTextLength; ++j) {
                    Vector2 previous = offset[link.linkTextfirstCharacterIndex + j];
                    offset[link.linkTextfirstCharacterIndex + j] = new Vector2((float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5));
                    var charInfo = info.characterInfo[link.linkTextfirstCharacterIndex + j];

                    int vertexIndex = charInfo.vertexIndex;
                    
                    vertices[vertexIndex + 0].x -= previous.x; vertices[vertexIndex + 0].y -= previous.y;
                    vertices[vertexIndex + 1].x -= previous.x; vertices[vertexIndex + 1].y -= previous.y;
                    vertices[vertexIndex + 2].x -= previous.x; vertices[vertexIndex + 2].y -= previous.y;
                    vertices[vertexIndex + 3].x -= previous.x; vertices[vertexIndex + 3].y -= previous.y;

                    Vector2 next = offset[link.linkTextfirstCharacterIndex + j];

                    vertices[vertexIndex + 0].x += next.x; vertices[vertexIndex + 0].y += next.y;
                    vertices[vertexIndex + 1].x += next.x; vertices[vertexIndex + 1].y += next.y;
                    vertices[vertexIndex + 2].x += next.x; vertices[vertexIndex + 2].y += next.y;
                    vertices[vertexIndex + 3].x += next.x; vertices[vertexIndex + 3].y += next.y;
                }
            }
            
            textMesh.mesh.vertices = vertices;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
