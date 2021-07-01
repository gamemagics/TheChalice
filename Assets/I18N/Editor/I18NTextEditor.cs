using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Akana {

    public class I18NTextEditor : EditorWindow {

        private static string _path;
        private static string _filename;
        private static TextFileParser parser;

        private ReorderableList _showList;

        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;

        private int _newKeyIndex = 0;

        [SerializeField]
        private List<KeyValuePair<string, string>> _pairList;

        [SerializeField]
        private List<string> _dummy = new List<string>();
        public static void openEditor(string path) {
            _path = path;
            parser = new TextFileParser(path);
            int index = _path.LastIndexOf("/");
            _filename = _path.Substring(index + 1);

            EditorWindow.GetWindow(typeof(I18NTextEditor));
        }

        private void OnEnable() {
            _pairList = parser.ToList();

            _serializedObject = new SerializedObject(this);
            _serializedProperty = _serializedObject.FindProperty("_dummy");
            _showList = new ReorderableList(_serializedObject, _serializedProperty, false, false, true, true);
            
            _showList.drawHeaderCallback = (Rect rect) => {
                GUI.Label(rect, "Text List");
            };

            _showList.drawElementCallback = (Rect rect, int index, bool selected, bool focus) => {
                KeyValuePair<string, string> pair = _pairList.ToArray()[index];

                Rect textRect = new Rect(rect) {
                    height = 32
                };
                string newKey = EditorGUI.TextField(textRect, pair.Key);
                if (newKey != pair.Key) {
                    parser.Replace(pair.Key, newKey);
                    _pairList.RemoveAt(index);
                    _pairList.Insert(index, new KeyValuePair<string, string>(newKey, pair.Value));
                }
                
                Rect areaRect = new Rect(rect) {
                    y = rect.y - 32,
                    height = 128
                };

                string newValue = EditorGUI.TextArea(areaRect, pair.Value);
                if (newValue != pair.Value) {
                    parser.SetString(pair.Key, newValue);
                    _pairList.RemoveAt(index);
                    _pairList.Insert(index, new KeyValuePair<string, string>(pair.Key, newValue));
                }
            };

            _showList.onAddCallback = (ReorderableList list) => {
                string key = "new key" + _newKeyIndex.ToString();
                while (parser.ContainsKey(key)) {
                    _newKeyIndex++;
                    key = "new key" + _newKeyIndex.ToString();
                }

                _newKeyIndex++;

                _pairList.Add(new KeyValuePair<string, string>(key, "new value"));
                parser.SetString(key, "new value");
            };

            _showList.onRemoveCallback = (ReorderableList list) => {
                int index = list.index;
                KeyValuePair<string, string> pair = _pairList.ToArray()[index];
                parser.Remove(pair.Key);
                _pairList.RemoveAt(index);
            };
        }

        private void OnGUI() {
            this.titleContent = new GUIContent(_filename);
            GUILayout.Label("Filename:");
            _filename = EditorGUILayout.TextField(_filename);

            if (GUILayout.Button("Rename")) {
                int index = _path.LastIndexOf("/");
                parser.Rename(_path.Substring(0, index + 1) + _filename);
            }

            EditorGUILayout.Space();

            _serializedObject.Update();
            _showList.DoLayoutList();
            _serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save")) {
                parser.Save();
            }
        }
    }

} // namespace Akana
