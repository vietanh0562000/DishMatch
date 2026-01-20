namespace PuzzleGames
{
    using System.Collections;
    using UnityEngine;

    public class InGameTracker : MonoBehaviour
    {
        public static int   TotalTimeSession = 0;
        public static int   NumBooster;
        public static float TotalPurchased;

        private bool endGame;

        public static void UseBooster()                  { NumBooster++; }
        public static void BuyPackageInGame(float price) { TotalPurchased += price; }

        public void StartTracking()
        {
            TotalTimeSession = 0;
            NumBooster       = 0;
            TotalPurchased   = 0;

            endGame = false;

            StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            while (endGame)
            {
                yield return new WaitForSeconds(1);
                TotalTimeSession++;
            }
        }

        public void StopTracking()
        {
            endGame = true;
            StopAllCoroutines();
        }
        public void ContinueTracking()
        {
            endGame = false;
            StartCoroutine(GameCoroutine());
        }
    }
}