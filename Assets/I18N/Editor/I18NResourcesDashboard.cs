using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Akana {
    /// <summary>
    /// Dashboard showing I18N resources in editor mode.
    /// </summary>
    public class I18NResourcesDashboard : EditorWindow {

        /// <summary>
        /// Current selected language
        /// </summary>
        private I18NLanguageCode _languageCode = I18NLanguageCode.EN_GB;

        /// <summary>
        /// List of filenames
        /// </summary>
        [SerializeField]
        private List<string> _fileList = new List<string>();

        /// <summary>
        /// Reorderable list object
        /// </summary>
        private ReorderableList _resourcesList;

        /// <summary>
        /// Serialized object
        /// </summary>
        private SerializedObject _serializedObject;

        /// <summary>
        /// Serialized property
        /// </summary>
        private SerializedProperty _serializedProperty;

        /// <summary>
        /// Index of new files' suffix
        /// </summary>
        private int _newFileIndex = 0;

        /// <summary>
        /// Show Dashboard window.
        /// </summary>
        [MenuItem("Akana/The Chalice/I18N Resources Dashboard")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(I18NResourcesDashboard));
        }

        /// <summary>
        /// Get the absolute path of current language directory.
        /// </summary>
        /// <returns>the absolute path of current language directory</returns>
        private string getSavedPath() {
            string path = Application.dataPath + @"/Resources/I18N/" + _languageCode.ToString();
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Function called by Unity when starting to render the window
        /// </summary>
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

        /// <summary>
        /// Add all yaml file in current language's directory into file list
        /// </summary>
        private void GetI18NFileList() {
            string path = getSavedPath();
            DirectoryInfo dir = new DirectoryInfo(path);
            _fileList.Clear();

            foreach (FileInfo file in dir.GetFiles("*.yaml")) {
                _fileList.Add(file.Name);
            }
        }

        /// <summary>
        /// Function called by Unity when starting to render the window
        /// </summary>
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
