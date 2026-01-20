using System.Collections;
using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace TRS.CaptureTool
{
    public class MultiLangScreenshotScript : ScreenshotSubComponentScript
    {
        public ScreenshotSeriesScript screenshotSeriesScript;

#if UNITY_LOCALIZATION
        // To receive language change updates use: LocalizationSettings.SelectedLocaleChanged
        public bool availableLocalesInSettings { get { return LocalizationSettings.AvailableLocales != null && LocalizationSettings.AvailableLocales.Locales != null && LocalizationSettings.AvailableLocales.Locales.Count > 0;  } }
        public bool allLocales = true;
        public Locale[] locales;
#else
        public static System.Action<SystemLanguage> LanguageChanged;
        public SystemLanguage[] languages;
#endif

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.MultiLanguage; } }
#endif

#pragma warning disable 0414
        [SerializeField]
        bool showTakeMultiLangCaptureButton = true;
        [SerializeField]
        bool showTakeMultiLangLiveFrameCaptureButton = true;
        [SerializeField]
        bool showTakeAllResolutionMultiLangCaptureButton = true;
        [SerializeField]
        bool showTakeAllResolutionMultiLangLiveFrameCaptureButton = true;
        [SerializeField]
        bool showTakeMultiLangSeriesCaptureButton = true;
        [SerializeField]
        bool showTakeMultiLangSeriesLiveFrameCaptureButton = true;
#pragma warning restore 0414

        protected override void Awake()
        {
            base.Awake();

            if (screenshotSeriesScript == null)
                screenshotSeriesScript = GetComponentInParent<ScreenshotSeriesScript>();
            if (screenshotSeriesScript == null)
                screenshotSeriesScript = GetComponentInChildren<ScreenshotSeriesScript>();

#if UNITY_LOCALIZATION
            if ((locales == null || locales.Length == 0) && LocalizationSettings.AvailableLocales != null)
                locales = LocalizationSettings.AvailableLocales.Locales.ToArray();
#endif
        }

        public void TakeMultiLangScreenshots(bool save = true)
        {
            StartCoroutine(TakeScreenshotsForLanguageCoroutine(screenshotScript.TakeSingleScreenshotCoroutine, save));
        }

        public void TakeMultiLangScreenshotsWithLiveFrames(bool save = true)
        {
            StartCoroutine(TakeScreenshotsForLanguageCoroutine(screenshotScript.TakeLiveFrameScreenshotsCoroutine, save));
        }

        public void TakeAllResolutionMultiLangScreenshots(bool save = true)
        {
            StartCoroutine(TakeScreenshotsForLanguageCoroutine(screenshotScript.TakeAllScreenshotsCoroutine, save));
        }

        public void TakeAllResolutionMultiLangScreenshotsWithLiveFrames(bool save = true)
        {
            StartCoroutine(TakeScreenshotsForLanguageCoroutine(screenshotScript.TakeAllScreenshotsWithLiveFramesCoroutine, save));
        }

        public void TakeMultiLangScreenshotSeries(bool save = true)
        {
            if (screenshotSeriesScript == null)
            {
                Debug.LogError("ScreenshotSeriesScript must be set to take screenshot series.");
                return;
            }

            StartCoroutine(TakeScreenshotsForLanguageCoroutine(screenshotSeriesScript.TakeScreenshotSeriesCoroutine, save));
        }

        public void TakeMultiLangScreenshotSeriesWithLiveFrames(bool save = true)
        {
            if (screenshotSeriesScript == null)
            {
                Debug.LogError("ScreenshotSeriesScript must be set to take screenshot series.");
                return;
            }

            ScreenshotScript.CaptureAndSaveRoutine takeScreenshotSeriesWithLiveFramesCoroutine = delegate(bool saveRoutine) {
                return screenshotSeriesScript.FlexibleTakeScreenshotSeriesCoroutine(screenshotScript.TakeAllScreenshotsWithLiveFramesCoroutine, saveRoutine);
	        };

            StartCoroutine(TakeScreenshotsForLanguageCoroutine(takeScreenshotSeriesWithLiveFramesCoroutine, save));
        }

        public IEnumerator TakeScreenshotsForLanguageCoroutine(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            bool hasValidSetup = HasValidSetup();
            if (!hasValidSetup) yield break;

            int originalCount = screenshotScript.fileSettings.count;
            bool originalScreenshotsInProgressOverride = screenshotScript.screenshotsInProgressOverride;
            bool originalIncludeLanguageInPath = screenshotScript.fileSettings.includeLanguageInPath;
            bool originalShouldOverrideLanguage = screenshotScript.fileSettings.shouldOverrideSystemLanguage;
            screenshotScript.fileSettings.shouldOverrideSystemLanguage = true;
            screenshotScript.fileSettings.includeLanguageInPath = true;
            screenshotScript.screenshotsInProgressOverride = true;

#if UNITY_LOCALIZATION
            Locale originalLocale = LocalizationSettings.SelectedLocale;
            Locale[] localesToUse = allLocales ? LocalizationSettings.AvailableLocales.Locales.ToArray() : locales;
            foreach (Locale locale in localesToUse)
            {
                if (locale == null) continue;
                LocalizationSettings.SelectedLocale = locale;
#else
            SystemLanguage originalLanguage = Application.systemLanguage;
            foreach (SystemLanguage language in languages)
            {
                SetSystemLanguage(language);
#endif

                screenshotScript.fileSettings.SetCount(originalCount);
                yield return StartCoroutine(captureAndSaveRoutine(save));
            }

#if UNITY_LOCALIZATION
            if(originalLocale != null)
                LocalizationSettings.SelectedLocale = originalLocale;
#else
            SetSystemLanguage(originalLanguage);
#endif

            screenshotScript.fileSettings.shouldOverrideSystemLanguage = originalShouldOverrideLanguage;
            screenshotScript.fileSettings.includeLanguageInPath = originalIncludeLanguageInPath;
            screenshotScript.screenshotsInProgressOverride = originalScreenshotsInProgressOverride;
        }

        bool HasValidSetup()
        {
#if UNITY_LOCALIZATION
            if(allLocales)
            {
                if(!availableLocalesInSettings)
                {
                    Debug.LogError("No available locales in LocalizationSettings to use for allLocales for multi-language screenshot script.");
                    return false;
                }
            } else if(locales == null || locales.Length <= 0)
            {
                Debug.LogError("No locales for are selected for multi-language screenshot script.");
                return false;
            }
#else
            if (LanguageChanged == null)
            {
                Debug.LogError("No listeners exist for language change event, so there would be no differences between screenshots.");
                return false;
            }

            if(languages.Length <= 0)
            {
                Debug.LogError("No languages for are selected for multi-language screenshot script.");
                return false;
            }
#endif

            return true;
        }

#if !UNITY_LOCALIZATION
        void SetSystemLanguage(SystemLanguage systemLanguage)
        {
            ScreenshotFileSettings.OverrideLanguage(systemLanguage);
            if (LanguageChanged != null)
                LanguageChanged(systemLanguage);
        }
#endif
    }
}