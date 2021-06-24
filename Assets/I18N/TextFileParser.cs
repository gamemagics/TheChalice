using System.Collections;
using System.Collections.Generic;
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
        private static readonly string _keyValueSplitter = ":";
        private static readonly char _quotes = '"';
        private static readonly char _escape = '\\';

        private Dictionary<string, string> _dictionary = null;
        public TextFileParser() {
        }
        
        public TextFileParser(string assetName) {
            Open(assetName);
        }

        public void Open(string assetName) {
            _dictionary = new Dictionary<string, string>();
            string content = Resources.Load<TextAsset>(_textDirectory + assetName).text;
            this.parseText(ref content);
        }

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
        public void SetString(string key, string val) {
            if (_dictionary == null) {
                _dictionary = new Dictionary<string, string>();
            }
    
            _dictionary[key] = val;
        }
    
        public void save(string assetName) {
            // TODO:
        }
#endif
    }

} // namespace Akana
