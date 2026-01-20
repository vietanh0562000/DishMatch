using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace SweetSugar.Scripts.Effects
{
    /// <summary>
    /// Explode animation for package item
    /// </summary>
    public class ExplAround : MonoBehaviour
    {
        [FormerlySerializedAs("item")] public MItem mItem;
        public FieldBoard field;
        public MItem[] items;
        public MItem[] itemsSecondCirle;
        public MItem[] itemsThirdCircle;
        public float duration = 1.2f;
        public AnimationCurve curve;
        public GameObject particle;
        public float startParticle = 0.25f;
        public AudioSource AudioSource;
        private void Play(MItem[] array, float delay, float force)
        {
            foreach (MItem item1 in array)
            {
                if(item1 == null) continue;
                item1.anim.enabled = false;
                var seq = LeanTween.sequence();
                Transform child = item1.transform.GetChild(0);
                Vector3 transformPosition = child.transform.position;
                Vector2 v = transformPosition + (transformPosition - mItem.transform.position).normalized * force;
                LeanTween.move(child.gameObject, v, duration / 2).setDelay(delay).setEase(curve).setOnComplete(OnFinished,child.gameObject);
            }

            LeanTween.delayedCall(5, Destr);
        }

        private void Destr()
        {
            if(this != null)
                Destroy(gameObject);
        }


        void OnFinished(object o)
        {
            MItem mItem = ((GameObject) o).transform.parent.GetComponent<MItem>();
            mItem.anim.enabled = true;//TODO: set destroying items animation false and true after
            mItem.ResetAnimTransform();
//            item.SetActive(false);
        }


        private void Start()
        {

            FillArrays();               
            Play(itemsSecondCirle,0.51f, 0.5f);
            Play(itemsThirdCircle,0.63f, 0.3f);
            LeanTween.delayedCall(startParticle, () => { particle.SetActive(true);});
            AudioSource.PlayDelayed(0.2f);
        }

        private void FillArrays()
        {
            if (mItem == null) return;
            items = GetItemsAround8(mItem.square).ToArray();
            itemsSecondCirle = GetItemsAroundSecond(mItem.square).ToArray();
            itemsThirdCircle = GetItemsAroundThird(mItem.square).ToArray();
        }

        private List<MItem> GetItemsAround8(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<MItem>
            {
                GetSquare(col + 0, row - 1, true)?.MItem,
                GetSquare(col + 1, row - 1, true)?.MItem,
                GetSquare(col + 1, row + 0, true)?.MItem,
                GetSquare(col + 1, row + 1, true)?.MItem,
                GetSquare(col + 0, row + 1, true)?.MItem,
                GetSquare(col - 1, row + 1, true)?.MItem,
                GetSquare(col - 1, row + 0, true)?.MItem,
                GetSquare(col - 1, row - 1, true)?.MItem
            };


            return itemsList;
        }

        private List<MItem> GetItemsAroundSecond(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<MItem>();

            var r = row - 2;
            var c = col - 2;
            for (c = col - 2; c <= col + 2; c++)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            c = col + 2;
            for (r = row - 1; r <= row + 2; r++)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            r = row + 2;
            for (c = col + 1; c >= col - 2; c--)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            c = col - 2;
            for (r = row + 1; r >= row - 1; r--)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            return itemsList;
        }

        private List<MItem> GetItemsAroundThird(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<MItem>();

            var r = row - 3;
            var c = col - 3;
            for (c = col - 3; c <= col + 3; c++)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            c = col + 3;
            for (r = row - 2; r <= row + 3; r++)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            r = row + 3;
            for (c = col + 2; c >= col - 3; c--)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            c = col - 3;
            for (r = row + 2; r >= row - 2; r--)
            {
                itemsList.Add(GetSquare(c, r, true)?.MItem);
            }

            return itemsList;
        }

        private Square GetSquare(int col, int row, bool safe = false)
        {
            if(!field)
            field = MLevelManager.THIS.field;
            return field.GetSquare(col, row);
        }

    }
}