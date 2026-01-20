using System;
using System.Collections;
using BasePuzzle.PuzzlePackages.Core;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    using BasePuzzle.PuzzlePackages.Core;

    [HideComponentField]
    public class IAPService : MonoCustomInspector
    {
        private const string _ENVIRONMENT = "production";
        private IAPService _instance;

        [Space(5), SerializeField, Tooltip("Phương thức dùng để xác thực thanh toán")]
        private ValidationMethod _validationMethod = ValidationMethod.ServerAndAppsflyer;

        [SerializeField, Min(3)] private float _timeout = 6f;

        [SerializeField, Tooltip("Tự động log doanh thu In-app Purchase lên Appsflyer.")]
        private bool _autoLogPurchase = true;

        [SerializeField] private bool _isSandbox;

        public float Timeout => _timeout;
        public ValidationMethod Validation => _validationMethod;
        private bool _isInitialized;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Bạn đã có 1 component IAPService khác ở trên Scene. Nên gameobject này sẽ bị destroy.");
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                Initialize();
            }
        }

        private IEnumerator Start()
        {
            //The AppsFlyerPurchaseConnector.init api initialized the connector for both iOS and Android.
            //However, the store parameter is only for Android stores.

            
            while (!_isInitialized)
            {
                yield return new WaitForSeconds(2);
            }
            
            InAppPurchaser.Init(this);
        }
        
        private void Initialize()
        {
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(_ENVIRONMENT);
                UnityServices.InitializeAsync(options).ContinueWith(_ => OnSuccess());
            }
            catch (Exception exception)
            {
                OnError(exception.Message);
            }
        }

        private void OnSuccess()
        {
            _isInitialized = true;
            Debug.Log(
                "IAPService > Unity Gaming Services has been successfully initialized.\nIn-App Purchasing is now initializing.");
        }

        private void OnError(string message)
        {
            Debug.LogError(
                $"IAPService > Unity Gaming Services failed to initialize with error: {message}.\nIn-App Purchasing functionality will not be available.");
        }

        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            InAppPurchaser.OnReceiveAppsflyerValidation(validationInfo);
        }

        private void OnDestroy()
        {
            InAppPurchaser.Dispose();
        }

#if UNITY_EDITOR
        [InspectorButton("Product Settings", 10, 5)]
        private void InAppPurchaseSettings()
        {
            if (!System.IO.Directory.Exists(ProductSettings.SETTINGS_NAME))
            {
                System.IO.Directory.CreateDirectory(ProductSettings.SETTINGS_PATH);
            }

            var settings = Resources.Load<ProductSettings>(ProductSettings.SETTINGS_NAME);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ProductSettings>();
                UnityEditor.AssetDatabase.CreateAsset(settings,
                    ProductSettings.SETTINGS_PATH + "/" + ProductSettings.SETTINGS_NAME + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.EditorUtility.FocusProjectWindow();
            }

            UnityEditor.Selection.activeObject = settings;
        }
#endif
    }
}