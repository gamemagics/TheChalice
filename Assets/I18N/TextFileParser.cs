using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Akana {

    /// <summary>
    /// I18N text file parser. The format of text is similar to yaml.
    /// </summary>
    public class TextFileParser {
        /// <summary>
        /// Directory that stores all I18N text files
        /// </summary>
        private static readonly string _textDirectory = "I18N/";

        /// <summary>
        /// Character used to split the key and value.
        /// </summary>
        private static readonly string _keyValueSplitter = ":";

        /// <summary>
        /// Quotes used to encapture the value string.
        /// </summary>
        private static readonly char _quotes = '"';

        /// <summary>
        /// The escape character.
        /// </summary>
        private static readonly char _escape = '\\';

        /// <summary>
        /// A dictionary storing the results of parsing.
        /// </summary>
        private Dictionary<string, string> _dictionary = null;
#if UNITY_EDITOR
        private string _filename;
#endif

        /// <summary>
        /// Initializer.
        /// </summary>
        public TextFileParser() {
        }
        
        /// <summary>
        /// Initializer.
        /// </summary>
        /// <param name="assetName">The name of resource which should be pareds.</param>
        public TextFileParser(string assetName, bool absolute = false) {
            Open(assetName, absolute);
        }

        /// <summary>
        /// Open a asset file. 
        /// If there is another file opened, the parser will close it.
        /// </summary>
        /// <param name="assetName">The name of parser.</param>
        public void Open(string assetName, bool absolute = false) {
            string content;
            if (absolute) {
                _filename = assetName;
                StreamReader reader = new StreamReader(assetName);
                content = reader.ReadToEnd();
                reader.Close();
            }
            else {
                content = Resources.Load<TextAsset>(_textDirectory + assetName).text;
            }
            
            _dictionary = new Dictionary<string, string>();
            this.parseText(ref content);
        }

        /// <summary>
        /// Parse the text resources and store them in the dictionary.
        /// </summary>
        /// <param name="text">The original text. Use reference to reduce the cost of copy.</param>
        private void parseText(ref string text) {
            int splitterPosition = text.IndexOf(_keyValueSplitter);
            while (splitterPosition > -1) {
                string key = text.Substring(0, splitterPosition);
                text = text.Substring(splitterPosition + 1);

                string val = "";
                int length = text.Length;
                int quotesCount = 0;
                for (int i = 0; i < length; ++i) {
                    if (quotesCount == 0) {
                        if (text[i] != _quotes) {
                            throw new System.Exception("quotes missing after colon, before" 
                                + text.Substring(i, Mathf.Min(5, length - i)) + "...");
                        }

                        ++quotesCount;
                    }
                    else if (quotesCount == 1) {
                        if (text[i] == _quotes && text[i - 1] != _escape) {
                            ++quotesCount;

                            if (i > 1) {
                                val = text.Substring(1, i - 1);
                                text = text.Substring(i + 1);
                            }
                            
                            break;
                        }
                    }
                }

                if (quotesCount < 2) {
                    throw new System.Exception("quotes missing before a new pair " 
                        + text.Substring(0, Mathf.Min(5, text.Length)) + "...");
                }

                _dictionary.Add(key, val);
                splitterPosition = text.IndexOf(_keyValueSplitter);
            }
        }

        /// <summary>
        /// Get string by given key.
        /// If the parser has never parsed a file before, or the key doesn't exist, it would return a default string.
        /// </summary>
        /// <param name="key">The given key.</param>
        /// <param name="defaultString">The default value</param>
        /// <returns>string, the result.</returns>
        public string GetString(string key, string defaultString = "") {
            if (_dictionary == null) {
                return defaultString;
            }
            
            string res = "";
            if (_dictionary.TryGetValue(key, out res)) {
                return res;
            } 
            else {
                return "";
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Save a string with spesified key.
        /// </summary>
        /// <param name="key">The key of string.</param>
        /// <param name="val">The string to be stored.</param>
        public void SetString(string key, string val) {
            if (_dictionary == null) {
                _dictionary = new Dictionary<string, string>();
            }
    
            _dictionary[key] = val;
        }

        public void Replace(string oldKey, string newKey) {
            if (_dictionary == null || !_dictionary.ContainsKey(oldKey)) {
                return;
            }

            _dictionary[newKey] = _dictionary[oldKey];
            _dictionary.Remove(oldKey);
        }
    
        public void Save() {
            StreamWriter writer = new StreamWriter(_filename);
            foreach (var pair in _dictionary) {
                writer.WriteLine(pair.Key + _keyValueSplitter + _quotes + pair.Value + _quotes);
            }

            writer.Close();
        }

        public void Rename(string newName) {
            File.Move(_filename, newName);
        }

        public List<KeyValuePair<string, string>> ToList() {
            var list = new List<KeyValuePair<string, string>>();
            foreach (var pair in _dictionary) {
                list.Add(pair);
            }

            return list;
        }

        public bool ContainsKey(string key) {
            return _dictionary != null && _dictionary.ContainsKey(key);
        }

        public void Remove(string key) {
            if (_dictionary == null || !_dictionary.ContainsKey(key)) {
                return;
            }

            _dictionary.Remove(key);
        }
#endif // #if UNITY_EDITOR
    }

} // namespace Akana
