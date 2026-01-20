using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    // Finalizers/Deconstructors (~ deinit methods) are not called consistently.
    // Therefore, the LanguageChanged method may get called multiple times.
    public class DemoLocalizationScript : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        [Tooltip("The ordered array of languages with translations.")]
        SystemLanguage[] languages = new SystemLanguage[0];
        [TextArea(1, 100)]
        [SerializeField]
        [Tooltip("The text that should be used for the language with the same index.")]
        string[] texts = new string[0];
#pragma warning restore 0649

        Dictionary<SystemLanguage, string> textForLanguage = new Dictionary<SystemLanguage, string>();

        Text text;

        public DemoLocalizationScript()
        {
#if !UNITY_LOCALIZATION
            MultiLangScreenshotScript.LanguageChanged += LanguageChanged;
#endif
        }

        ~DemoLocalizationScript()
        {
#if !UNITY_LOCALIZATION
            MultiLangScreenshotScript.LanguageChanged -= LanguageChanged;
#endif
        }

        void Awake()
        {
            Setup();
        }

        void Setup()
        {
            if (text == null)
                text = GetComponent<Text>();
            if (text == null)
                Debug.LogError("No text component found.");

            MergeArraysToDictionary();
        }

        void MergeArraysToDictionary()
        {
            int fullPairCount = languages.Length;
            if (languages.Length > texts.Length)
                Debug.LogError("Missing text for language.");
            else if (languages.Length < texts.Length)
            {
                fullPairCount = texts.Length;
                Debug.LogError("Missing language for text.");
            }

            textForLanguage = new Dictionary<SystemLanguage, string>();
            for (int i = 0; i < fullPairCount; ++i)
                textForLanguage[languages[i]] = texts[i];
        }

        void LanguageChanged(SystemLanguage language)
        {
            if (this == null)
            {
#if !UNITY_LOCALIZATION
                MultiLangScreenshotScript.LanguageChanged -= LanguageChanged;
#endif
                return;
            }

            Setup();
            text.text = textForLanguage[language];
        }
    }
}