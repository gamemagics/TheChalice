using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

namespace Akana {
    public class I18NResourcesDashboard : EditorWindow {

        private I18NLanguageCode _languageCode = I18NLanguageCode.EN_GB;
        [SerializeField]
        private List<string> _fileList = new List<string>();

        private ReorderableList _resourcesList;

        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;

        [MenuItem("Akana/The Chalice/I18N Resources Dashboard")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(I18NResourcesDashboard));
        }

        private void OnEnable() {
            GetI18NFileList();

            _serializedObject = new SerializedObject(this);
            _serializedProperty = _serializedObject.FindProperty("_fileList");
            _resourcesList = new ReorderableList(_serializedObject, _serializedProperty, false, false, true, true);

            _resourcesList.drawHeaderCallback = (Rect rect) => {
                GUI.Label(rect, "Resources List");
            };

            _resourcesList.drawElementCallback = (Rect rect, int index, bool selected, bool focus) => {
                SerializedProperty item = _resourcesList.serializedProperty.GetArrayElementAtIndex(index);

                Rect labelRect = new Rect(rect) {
                    width = rect.width - 64
                };
                EditorGUI.LabelField(labelRect, item.stringValue);
                
                Rect buttonRect = new Rect(rect) {
                    width = 36,
                    x = rect.x + rect.width - 36
                };
                GUI.Button(buttonRect, "Edit");
            };
        }

        private void GetI18NFileList() {
            string path = Application.dataPath + @"/Resources/I18N/" + _languageCode.ToString();
            DirectoryInfo dir = new DirectoryInfo(path);
            _fileList.Clear();

            foreach (FileInfo file in dir.GetFiles("*.yaml")) {
                _fileList.Add(file.Name);
            }
        }

        private void OnGUI() {
            this.titleContent = new GUIContent("I18N Resources Dashboard");

            _languageCode = (I18NLanguageCode)EditorGUILayout.EnumPopup("Language", _languageCode);
            EditorGUILayout.Space();
            if (GUILayout.Button("Load")) {
                GetI18NFileList();
            }

            EditorGUILayout.Space();

            _serializedObject.Update();
            _resourcesList.DoLayoutList();
            _serializedObject.ApplyModifiedProperties();
        }
    }
} // namespace Akana
