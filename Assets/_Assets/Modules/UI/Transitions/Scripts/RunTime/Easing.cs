using UnityEngine;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    public static class Easing
    {
        public static float OutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3);
        }

        public static float InBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * x * x * x - c1 * x * x;
        }

        public static float OutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2);
        }
    }
}