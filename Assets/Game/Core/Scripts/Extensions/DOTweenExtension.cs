using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace BasePuzzle
{
    public static class DOTweenExtension
    {
        #region Regular Text

        public static Tweener DOTextInt(this Text text, int initialValue, int finalValue, float duration, Func<int, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextInt(this Text text, int initialValue, int finalValue, float duration)
        {
            return DOTweenExtension.DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());
        }

        public static Tweener DOTextFloat(this Text text, float initialValue, float finalValue, float duration, Func<float, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextFloat(this Text text, float initialValue, float finalValue, float duration)
        {
            return DOTweenExtension.DOTextFloat(text, initialValue, finalValue, duration, it => it.ToString());
        }

        public static Tweener DOTextLong(this Text text, long initialValue, long finalValue, float duration, Func<long, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextLong(this Text text, long initialValue, long finalValue, float duration)
        {
            return DOTweenExtension.DOTextLong(text, initialValue, finalValue, duration, it => it.ToString());
        }

        public static Tweener DOTextDouble(this Text text, double initialValue, double finalValue, float duration, Func<double, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextDouble(this Text text, double initialValue, double finalValue, float duration)
        {
            return DOTweenExtension.DOTextDouble(text, initialValue, finalValue, duration, it => it.ToString());
        }

        #endregion

        //==========================================================================================================================

        #region Text TMP

        public static Tweener DOTextInt(this TextMeshProUGUI text, int initialValue, int finalValue, float duration, Func<int, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextInt(this TextMeshProUGUI text, int initialValue, int finalValue, float duration, string extension = "", bool isHeadExtension = false)
        {
            if (string.IsNullOrEmpty(extension))
            {
                Func<int, string> conv = it => it.ToString();
                return DOTweenExtension.DOTextInt(text, initialValue, finalValue, duration, conv);
            }
            else
            {
                Func<int, string> conv = it => (it.ToString() + extension);
                
                if (isHeadExtension)
                {
                    conv = it => (extension + it.ToString());
                }

                return DOTweenExtension.DOTextInt(text, initialValue, finalValue, duration, conv);
            }
        }

        public static Tweener DOTextInt(this TextMeshPro text, int initialValue, int finalValue, float duration, string extension = "", bool isHeadExtension = false)
        {
            if (string.IsNullOrEmpty(extension))
            {
                Func<int, string> conv = it => it.ToString();
                return text.DOTextInt(initialValue, finalValue, duration, conv);
            }
            else
            {
                Func<int, string> conv = it => (it.ToString() + extension);
                
                if (isHeadExtension)
                {
                    conv = it => (extension + it.ToString());
                }

                return text.DOTextInt(initialValue, finalValue, duration, conv);
            }
        }
        
        public static Tweener DOTextInt(this TextMeshPro text, int initialValue, int finalValue, float duration, Func<int, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }


        public static Tweener DOTextFloat(this TextMeshProUGUI text, float initialValue, float finalValue, float duration, Func<float, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextFloat(this TextMeshProUGUI text, float initialValue, float finalValue, float duration, string extension = "")
        {
            if (string.IsNullOrEmpty(extension))
            {
                Func<float, string> conv = it => it.ToString("F1");
                return DOTweenExtension.DOTextFloat(text, initialValue, finalValue, duration, conv);
            }
            else
            {
                Func<float, string> conv = it => (it.ToString("F1") + extension);
                return DOTweenExtension.DOTextFloat(text, initialValue, finalValue, duration, conv);
            }
        }

        public static Tweener DOTextLong(this TextMeshProUGUI text, long initialValue, long finalValue, float duration, Func<long, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextLong(this TextMeshProUGUI text, long initialValue, long finalValue, float duration)
        {
            return DOTweenExtension.DOTextLong(text, initialValue, finalValue, duration, it => it.ToString());
        }

        public static Tweener DOTextDouble(this TextMeshProUGUI text, double initialValue, double finalValue, float duration, Func<double, string> convertor)
        {
            return DOTween.To(
                () => initialValue,
                it => text.text = convertor(it),
                finalValue,
                duration
            );
        }

        public static Tweener DOTextDouble(this TextMeshProUGUI text, double initialValue, double finalValue, float duration)
        {
            return DOTweenExtension.DOTextDouble(text, initialValue, finalValue, duration, it => it.ToString());
        }

        #endregion
    }
}