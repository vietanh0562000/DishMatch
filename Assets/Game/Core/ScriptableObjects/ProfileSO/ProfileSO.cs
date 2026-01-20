namespace PuzzleGames
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [CreateAssetMenu(fileName = "ProfileCollection", menuName = "ScriptableObjects/Profile Collection")]
    public class ProfileSO : ScriptableObject
    {
        [SerializeField] private ListDataSo<Sprite> avatars, frames;
        
        public ListDataSo<Sprite> Avatars => avatars;
        public ListDataSo<Sprite> Frames => frames;
    }
}