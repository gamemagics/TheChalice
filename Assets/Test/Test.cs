using Akana;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    public Text showText = null;
    private I18NHelper _helper = null;

    // Start is called before the first frame update
    void Start() {
        _helper = I18NHelper.getInstance();
        _helper.useAsset("test");
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ChangeLanguage(string lang) {
        I18NLanguageCode code = (I18NLanguageCode)Enum.Parse(typeof(I18NLanguageCode), lang);
        _helper.setLanguageCode(code);
        string str = _helper.getStringByKey("hello", "ERROR");
        showText.text = str;
    }
}
