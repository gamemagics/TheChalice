using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StaticUI : MonoBehaviour {
    public string sectionName;

    [SerializeField] private TMPro.TMP_Text tmpText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetText(string text) {
        tmpText.text = text;
    }
}
