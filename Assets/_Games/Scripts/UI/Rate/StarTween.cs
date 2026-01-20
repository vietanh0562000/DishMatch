namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class StarTween : MonoBehaviour
    {
        public Image[] stars;

        public void Init()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                Image star = stars[i];
                star.transform.localScale = Vector3.zero;
                star.color                = new Color(1, 1, 1, 0); // invisible
            }
        }

        public float delayBetweenStars = 0.2f;
        public float appearDuration    = 0.3f;

        public void ShowStars()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                Image star = stars[i];

                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(i * delayBetweenStars);
                seq.Append(star.DOFade(1f, appearDuration * 0.5f));
                seq.Join(star.transform.DOScale(1.2f, appearDuration).SetEase(Ease.OutBack));
                seq.Append(star.transform.DOScale(1f, 0.2f));
            }
        }
    }
}