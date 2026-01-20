using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Messages
{
    /// <summary>
    /// Used by the messenger to hook into the unity update process. This allows us
    /// to delay messages instead of sending them right away. We'll release messages
    /// if a new level is loaded.
    /// </summary>
    public sealed class MessageDispatcherStub : MonoBehaviour
    {
        /// <summary>
        /// Raised first when the object comes into existance. Called
        /// even if script is not enabled.
        /// </summary>
        void Awake()
        {
            // Don't destroyed automatically when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Update is called every frame. We pass this to our messenger
        /// </summary>
        void Update()
        {
            MessageDispatcher.Update();
        }

        /// <summary>
        /// Called when the dispatcher is disabled. We use this to
        /// clean up the event tables everytime a new level loads.
        /// </summary>
        public void OnDisable()
        {
            MessageDispatcher.ClearMessages();
            MessageDispatcher.ClearListeners();
        }
    }
}
