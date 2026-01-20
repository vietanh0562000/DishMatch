using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SweetSugar.Scripts.GUI
{
    /// <summary>
    /// Score progress bar handler
    /// </summary>
    public class ProgressBarScript : MonoBehaviour
    {
        Image slider;
        public static ProgressBarScript Instance;
        float maxWidth;
        public GameObject[] stars;
        static bool[] starsAwarded = new bool[3];
        // Use this for initialization
        void OnEnable()
        {
            Instance = this;
            slider = GetComponent<Image>();
            maxWidth = 1;
            MLevelManager.OnLevelLoaded += InitBar;
            if(MLevelManager.GetGameStatus() > GameState.PrepareGame)
                InitBar();
        }

        private void OnDisable()
        {
            MLevelManager.OnLevelLoaded -= InitBar;

        }

        public void InitBar()
        {
            ResetBar();
            PrepareStars();

        }

        public void UpdateDisplay(float x)
        {
            slider.fillAmount = maxWidth * x;
            if (maxWidth * x >= maxWidth)
            {
                slider.fillAmount = maxWidth;

                //	ResetBar();
            }
            CheckStars();
        }

        public void AddValue(float x)
        {
            UpdateDisplay(slider.fillAmount * 100 / maxWidth / 100 + x);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateDisplay(MLevelManager.Score * 100f / (MLevelManager.THIS.mLevelData.star1 / ((MLevelManager.THIS.mLevelData.star1 * 100f / MLevelManager.THIS.mLevelData.star3)) * 100f) / 100f);
        }

        public bool IsFull()
        {
            if (slider.fillAmount >= maxWidth)
            {
                ResetBar();
                return true;
            }

            return false;
        }

        public void ResetBar()
        {
            UpdateDisplay(0.0f);
        }

        void PrepareStars()
        {
            if (MLevelManager.THIS != null && MLevelManager.THIS?.mLevelData != null)
            {
                var width = GetComponent<RectTransform>().rect.width;
                stars[0].transform.localPosition = new Vector3(MLevelManager.THIS.mLevelData.star1 * 100f / MLevelManager.THIS.mLevelData.star3 * width / 100 - (width / 2f), stars[0].transform.localPosition.y, 0);
                stars[1].transform.localPosition = new Vector3(MLevelManager.THIS.mLevelData.star2 * 100f / MLevelManager.THIS.mLevelData.star3 * width / 100 - (width / 2f), stars[1].transform.localPosition.y, 0);
                stars[0].transform.GetChild(0).gameObject.SetActive(false);
                stars[1].transform.GetChild(0).gameObject.SetActive(false);
                stars[2].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        void CheckStars()
        {
            var star1Anim = stars[0].transform.GetChild(0).gameObject;
            var star2Anim = stars[1].transform.GetChild(0).gameObject;
            var star3Anim = stars[2].transform.GetChild(0).gameObject;
            var Score = MLevelManager.Score;
            var levelData = MLevelManager.THIS?.mLevelData;
            if (levelData == null) return;

            if (Score >= levelData.star1)
            {
                if (!star1Anim.activeSelf && !starsAwarded[0])
                    SoundBase.Instance.PlayOneShot(SoundBase.Instance.getStarIngr);
                star1Anim.SetActive(true);
                starsAwarded[0] = true;
            }
            if (Score >= levelData.star2)
            {
                if (!star2Anim.activeSelf && !starsAwarded[1])
                    SoundBase.Instance.PlayOneShot(SoundBase.Instance.getStarIngr);
                star2Anim.SetActive(true);
                starsAwarded[1] = true;

            }
            if (Score >= levelData.star3)
            {
                if (!star3Anim.activeSelf && !starsAwarded[2])
                    SoundBase.Instance.PlayOneShot(SoundBase.Instance.getStarIngr);
                star3Anim.SetActive(true);
                starsAwarded[2] = true;

            }
        }
    }
}
