namespace PuzzleGames
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MechanicVisual : MonoBehaviour
    {
        [SerializeField] private Image    iconImage; // UI Image to display the mechanic's icon
        [SerializeField] private TMP_Text nameText; // UI Text to display the mechanic's name
        [SerializeField] private TMP_Text detailText; // UI Text to display the mechanic's detail

        /// <summary>
        /// Sets the visual data from a given MechanicData.
        /// </summary>
        /// <param name="mechanicData">The mechanic data to visualize.</param>
        public void SetVisual(MechanicData mechanicData)
        {
            if (iconImage)
            {
                iconImage.sprite = mechanicData.Icon; // Set the visual icon
                iconImage.SetNativeSize();
            }

            if (nameText)
                nameText.text = mechanicData.Name; // Set the name text

            if (detailText)
                detailText.text = mechanicData.Detail; // Set the detail/description text
        }
    }
}