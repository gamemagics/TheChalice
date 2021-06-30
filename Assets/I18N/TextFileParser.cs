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

        /// <summary>
        /// Initializer.
        /// </summary>
        public TextFileParser() {
        }
        
        /// <summary>
        /// Initializer.
        /// </summary>
        /// <param name="assetName">The name of resource which should be pareds.</param>
        public TextFileParser(string assetName) {
            Open(assetName);
        }

        /// <summary>
        /// Open a asset file. 
        /// If there is another file opened, the parser will close it.
        /// </summary>
        /// <param name="assetName">The name of parser.</param>
        public void Open(string assetName) {
            _dictionary = new Dictionary<string, string>();
            string content = Resources.Load<TextAsset>(_textDirectory + assetName).text;
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
    
        public void save(string assetName) {
            // TODO:
        }
#endif // #if UNITY_EDITOR
    }

} // namespace Akana
