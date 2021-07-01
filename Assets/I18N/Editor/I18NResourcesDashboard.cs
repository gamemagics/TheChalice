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

        private int _newFileIndex = 0;

        [MenuItem("Akana/The Chalice/I18N Resources Dashboard")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(I18NResourcesDashboard));
        }

        private string getSavedPath() {
            string path = Application.dataPath + @"/Resources/I18N/" + _languageCode.ToString();
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            return path;
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

                if (GUI.Button(buttonRect, "Edit")) {
                    string path = getSavedPath() + "/" + _fileList.ToArray()[index];
                    I18NTextEditor.openEditor(path);
                }
            };

            _resourcesList.onAddCallback = (ReorderableList list) => {
                string filename = "NewI18NTextFile" + _newFileIndex.ToString() + ".yaml";
                string path = getSavedPath() + "/" + filename;
                while (File.Exists(path)) {
                    _newFileIndex++;
                    filename = "NewI18NTextFile" + _newFileIndex.ToString() + ".yaml";
                    path = getSavedPath() + "/" + filename;
                }

                _fileList.Add(filename);
                _newFileIndex++;
                FileStream stream = new FileStream(path, FileMode.CreateNew);
                stream.Close();
            };

            _resourcesList.onRemoveCallback = (ReorderableList list) => {
                int index = list.index;
                string path = getSavedPath() + "/" + _fileList.ToArray()[index];
                _fileList.RemoveAt(list.index);

                if (File.Exists(path)) {
                    File.Delete(path);
                }
            };
        }

        private void GetI18NFileList() {
            string path = getSavedPath();
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
