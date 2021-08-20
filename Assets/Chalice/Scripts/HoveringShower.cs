using UnityEngine;

namespace Akana {

public class HoveringShower : MonoBehaviour {
    [SerializeField] private GameObject dialogue;
    [SerializeField] private Vector2 padding;

    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private Canvas canvas;

    void Awake() {
        if (dialogue == null) {
            Debug.LogError("Empty Dialoue Object!");
            return;
        }

        dialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnMouseEnter() {
        AdjustPosition();
        dialogue.SetActive(true);
    }

    private void OnMouseExit() {
        dialogue.SetActive(false);
    }

    private bool TestCorner(Camera camera, Vector3[] corner) {
        var vp = camera.cullingMatrix;
        bool flag = true;
        foreach (var v in corner) {
            var vec4 = new Vector4(v.x, v.y, v.z, 1);
            var p = vp * vec4;

            flag &= (p.w > p.x && -p.w < p.x && p.w > p.y && -p.w < p.y && p.w > p.z && -p.w < p.z);
        }
        
        return flag;
    }

    private void AdjustPosition() {
        Vector3[] corners = new Vector3[4];

        var rect = dialogue.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, boxCollider.size.y + padding.y, 0);
        rect.GetWorldCorners(corners);
        if (TestCorner(Camera.main, corners)) {
            return;
        }

        rect.localPosition = new Vector3(-boxCollider.size.x - padding.x, 0, 0);
        rect.GetWorldCorners(corners);
        if (TestCorner(Camera.main, corners)) {
            return;
        }

        rect.localPosition = new Vector3(0, -boxCollider.size.y - padding.y, 0);
        rect.GetWorldCorners(corners);
        if (TestCorner(Camera.main, corners)) {
            return;
        }

        rect.localPosition = new Vector3(boxCollider.size.x + padding.x, 0, 0);
    }
}

} // namespace Akana
