using UnityEngine;
using Yarn;
using Yarn.Unity;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using System.Collections;
using System;

[AddComponentMenu("Scripts/Yarn Spinner/Static UI Runner")]
public class StaticUIRunner : MonoBehaviour {
    [SerializeField] private YarnProgram script;
    [SerializeField] private string textLanguage;

    [SerializeField] private VariableStorageBehaviour variableStorage;

    [SerializeField] private StaticUI[] children;

    private MapField<string, Node> mapField;

    private Dictionary<string, string> ids = new Dictionary<string, string>();

    private Dictionary<string, string> texts;

    // Start is called before the first frame update
    void Awake() {
        if (variableStorage != null) {
            variableStorage.ResetToDefaults();
        }

        if (script != null) {
            Program program = script.GetProgram();
            mapField = program.Nodes;

            foreach (var pair in mapField) {
                string line = pair.Value.Instructions[0].Operands[0].StringValue;
                ids.Add(pair.Key, line);
            }
        }
    }

    void Start() {
        UpdateTexts();
        UpdateComponents();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void updateLanguage(string lang) {
        if (textLanguage != lang) {
            textLanguage = lang;
            UpdateTexts();
            UpdateComponents();
        }
    }

    private void UpdateTexts() {
        texts = new Dictionary<string, string>();
        var textAsset = new TextAsset();
        
        if (script.localizations != null && script.localizations.Length > 0) {
            textAsset = Array.Find(script.localizations, elements => elements.languageName == textLanguage)?.text;
        }

        if (textAsset == null || string.IsNullOrEmpty(textAsset.text)) {
            textAsset = script.baseLocalisationStringTable;
        }

        var configuration = new CsvHelper.Configuration.Configuration(System.Globalization.CultureInfo.InvariantCulture);
        
        using (var reader = new System.IO.StringReader(textAsset.text)) {
            using (var csv = new CsvHelper.CsvReader(reader, configuration)) {
                csv.Read(); csv.ReadHeader();

                while (csv.Read()) {
                    texts.Add(csv.GetField("id"), csv.GetField("text"));
                }
            }
        }
    }

    private void UpdateComponents() {
        foreach (var child in children) {
            if (ids.ContainsKey(child.sectionName)) {
                string id = ids[child.sectionName];
                if (texts.ContainsKey(id)) {
                    string text = texts[id];
                    child.SetText(text);
                }
            }
        }
    }
}
