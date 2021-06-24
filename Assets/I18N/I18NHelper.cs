using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akana {

    public class I18NHelper {
        private static I18NHelper _instance = null;
        private string _assetName = null;

        private I18NLanguageCode _code;
        private TextFileParser _parser;

        private I18NHelper() {
            _code = I18NLanguageCode.EN_GB;
            _parser = new TextFileParser();
        }

        public static I18NHelper getInstance() {
            if (_instance == null) {
                _instance = new I18NHelper();
            }

            return _instance;
        }

        public void setLanguageCode(I18NLanguageCode code) {
            if (_code != code) {
                _code = code;
                if (_assetName != null) {
                    _parser.Open(_code.ToString() + "/" + _assetName);
                }
            }
        }

        public void useAsset(string assetName) {
            if (assetName != _assetName) {
                _assetName = assetName;
                _parser.Open(_code.ToString() + "/" + assetName);
            }
        }

        public string getStringByKey(string key, string defaultValue = "") {
            return _parser.GetString(key, defaultValue);
        }
    }

} // namespace Akana
