using System;
using System.Collections;
using BasePuzzle.Core.Scripts.Controllers.Interfaces;
using BasePuzzle.Core.Scripts.Exceptions;
using BasePuzzle.Core.Scripts.Logs;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using BasePuzzle.Core.Scripts.Utils.Entities;
using BasePuzzle.Core.Scripts.Utils.Generics;
using BasePuzzle.Core.Scripts.Utils.Sequences.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace BasePuzzle.Core.Scripts
{
    using BasePuzzle.Core.Scripts.Controllers.Interfaces;
    using BasePuzzle.Core.Scripts.Exceptions;
    using BasePuzzle.Core.Scripts.Logs;
    using BasePuzzle.Core.Scripts.Services.GameObjs;
    using BasePuzzle.Core.Scripts.Utils.Entities;
    using BasePuzzle.Core.Scripts.Utils.Generics;
    using BasePuzzle.Core.Scripts.Utils.Sequences.Core;

    public class GameMain : MonoBehaviour
    {
        public static ExecState InitState { get; private set; } = ExecState.NotStarted;
        public static bool InitComplete => InitState == ExecState.Succeed;
        public static event EventHandler OnInitComplete;

        /// <summary>
        ///     Initialize the SDk
        /// </summary>
        /// <remarks>
        ///     From SDK version 2.2.0, this function is no longer needed to be called manually by the user
        /// </remarks>
        /// <exception cref="FSdkException">If function not called from the main thread, only be thrown if running in editor</exception>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (ExecStates.CanStart(InitState))
            {
#if UNITY_EDITOR
                if (!InternalEditorUtility.CurrentThreadIsMainThread())
                    throw new FSdkException("FalconMain.Init() can only be called from the main thread");
#endif
                FGameObj.Instance.StartCoroutine(new SequenceWrap(InitIEnumerator(), e =>
                {
                    CoreLogger.Instance.Error(e);
                    InitState = ExecState.Failed;
                }));
            }
        }

        private static IEnumerator InitIEnumerator()
        {
            InitState = ExecState.Processing;
            CoreLogger.Instance.Info("Sdk Initialize Started");

            var inits = FGenerics.GetInstances<IFInit>();

            foreach (var fInit in inits)
            {
                CoreLogger.Instance.Info(fInit.GetType() + " initializing");
                yield return fInit.Init();
                CoreLogger.Instance.Info(fInit.GetType() + " init complete");
            }

            CoreLogger.Instance.Info("Initialize complete");

            InitState = ExecState.Succeed;
            OnInitComplete?.Invoke(null, EventArgs.Empty);
        }
    }
}