namespace Akana {

    /// <summary>
    /// Singleton I18N Helper Class. Use this class instead access I18N text resources directly.
    /// </summary>
    public class I18NHelper {
        /// <summary>
        /// Static instance. Default language is English.
        /// </summary>
        private static I18NHelper _instance = null;
        /// <summary>
        /// The name of text file being used now.
        /// </summary>
        private string _assetName = null;

        /// <summary>
        /// Current language's code
        /// </summary>
        private I18NLanguageCode _code;

        /// <summary>
        /// Text resources parser.
        /// </summary>
        private TextFileParser _parser;

        /// <summary>
        /// Private initializer.
        /// </summary>
        private I18NHelper() {
            _code = I18NLanguageCode.EN_GB;
            _parser = new TextFileParser();
        }

        /// <summary>
        /// Get the instance of helper.
        /// </summary>
        /// <returns>I18NHelper, the instance of helper.</returns>
        public static I18NHelper getInstance() {
            if (_instance == null) {
                _instance = new I18NHelper();
            }

            return _instance;
        }

        /// <summary>
        /// Use resources in another language. 
        /// If you have opened a text file before, it will automatically find the resource file in new language.
        /// </summary>
        /// <param name="code">The new language's code</param>
        public void setLanguageCode(I18NLanguageCode code) {
            if (_code != code) {
                _code = code;
                if (_assetName != null) {
                    _parser.Open(_code.ToString() + "/" + _assetName);
                }
            }
        }

        /// <summary>
        /// Use another text resource in current language.
        /// </summary>
        /// <param name="assetName">The name of text file.</param>
        public void useAsset(string assetName) {
            if (assetName != _assetName) {
                _assetName = assetName;
                _parser.Open(_code.ToString() + "/" + assetName);
            }
        }

        /// <summary>
        /// Get text value by given key. 
        /// If you have not yet opened a file, or the key doesn't exist, a default value would be returned.
        /// </summary>
        /// <param name="key">The given key</param>
        /// <param name="defaultValue">The default value returned if can't find the text</param>
        /// <returns>string, the result.</returns>
        public string getStringByKey(string key, string defaultValue = "") {
            return _parser.GetString(key, defaultValue);
        }
    }

} // namespace Akana
