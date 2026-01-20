using SweetSugar.Scripts.Core;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Game blocks handler. It lock ups the game on animations
    /// </summary>
    public class MGameBlocker : UnityEngine.MonoBehaviour
    {
        private void Start()
        {
            MLevelManager.THIS._stopFall.Add(this);
        }

        private void OnDisable()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            MLevelManager.THIS._stopFall.Remove(this);
        }
    }
}