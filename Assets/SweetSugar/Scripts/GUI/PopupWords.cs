using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Greetings words for a combo
    /// </summary>
    public class PopupWords : MonoBehaviour
    {
        private void Update()
        {
            transform.position = MLevelManager.THIS.field.GetPosition();
        }
    }
}