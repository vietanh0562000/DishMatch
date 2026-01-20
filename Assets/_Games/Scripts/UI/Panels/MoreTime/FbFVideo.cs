namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FbFVideo : MonoBehaviour
    {
        public Sprite[] frames; // Array of sprite frames
        public Image    targetImage; // Reference to the UI Image component where sprites will be shown
        public float    frameRate = 12f; // Frames per second

        private int   currentFrame;
        private float timer;

        void Update()
        {
            if (frames == null || frames.Length == 0 || targetImage == null)
                return;

            timer += Time.unscaledDeltaTime;
            if (timer >= 1f / frameRate)
            {
                currentFrame       = (currentFrame + 1) % frames.Length;
                targetImage.sprite = frames[currentFrame];
                timer              = 0f;
            }
        }
    }
}