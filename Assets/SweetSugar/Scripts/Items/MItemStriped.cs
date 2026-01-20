using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Striped item
    /// </summary>
    public class MItemStriped : MItem, IItemInterface
    {
        private MItem _mItemMain;
        // public bool Combinable;
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        private MItem GetMItem
        {
            get { return GetComponentInParent<MItem>(); }
        }

        public void Destroy(MItem item1, MItem item2)
        {
            //        if (GetItem.dontDestroyOnThisMove ) return;
            //        GetItem.SetDontDestroyOnMove();
            //        Debug.Log(" pre destroy " + " type " + GetItem.currentType + GetItem.GetInstanceID() + " " + item1?.GetInstanceID() + " : " + item2?.GetInstanceID());
            GetMItem.square.DestroyBlock();
            if (GetMItem.square.type == SquareTypes.WireBlock)
            {
                GetMItem.StopDestroy();
                return;
            }
            var list = new[] { item1, item2 };
            list = list.OrderBy(i => i != GetMItem).ToArray();
            item1 = list.First();
            item2 = list.Last();

            _mItemMain = item1;
            var square = _mItemMain.square;
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.strippedExplosion);
            MLevelManager.THIS.StripedShow(gameObject, item1.currentType == ItemsTypes.HORIZONTAL_STRIPED);
            var itemsList = GetList(square);
            foreach (var item in itemsList)
            {
                if (item != null)
                {
                    item.DestroyItem(true, GetMItem, this);
                }
            }

            var sqL = GetSquaresInRow(square, _mItemMain.currentType);
            square.DestroyBlock();
            if(square.type == SquareTypes.JellyBlock)
                MLevelManager.THIS.mLevelData.GetTargetObject().CheckSquares(sqL.ToArray());
            sqL.Where(i => i.mItem == null).ToList().ForEach(i => i.DestroyBlock());

            DestroyBehaviour();

        }

        private List<Square> GetSquaresInRow(Square square, ItemsTypes type)
        {
            if (type == ItemsTypes.HORIZONTAL_STRIPED)
                return MLevelManager.THIS.GetRowSquare(square.row);
            return MLevelManager.THIS.GetColumnSquare(square.col);
        }

        public MItem GetParentItem()
        {
            return transform.GetComponentInParent<MItem>();
        }

        private List<MItem> GetList(Square square)
        {
            if (_mItemMain.currentType == ItemsTypes.HORIZONTAL_STRIPED)
                return MLevelManager.THIS.GetRow(square.row);
            return MLevelManager.THIS.GetColumn(square.col);
        }

        public override void Check(MItem item1, MItem item2)
        {
            CheckStripes(item1, item2);
            if(gameObject.activeSelf)
                StartCoroutine(CheckStripePackage(item1, item2));

            if (item2.currentType == ItemsTypes.MARMALADE || item2.currentType == ItemsTypes.MULTICOLOR)
            {
                item2.Check(item2, item1);
            }
        }

        /// <summary>
        /// Check if package and striped switched
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private IEnumerator CheckStripePackage(MItem item1, MItem item2)
        {
            MItem[] list = { item1, item2 };
            var striped = list.Where(i =>
                    i.currentType == ItemsTypes.HORIZONTAL_STRIPED || i.currentType == ItemsTypes.VERTICAL_STRIPED)
                ?.FirstOrDefault();
            var package = list.Where(i => i.currentType == ItemsTypes.PACKAGE)?.FirstOrDefault();

            if (striped != null && package != null)
            {
                var itemsList = MLevelManager.THIS.GetSquaresAroundSquare(package.square);
                var direction = 1;
                itemsList.Add(item1.square);
                List<Square> squares = new List<Square>();
                foreach (var _square in itemsList)
                {
                    if (_square != null)
                    {
                        var list1 = GetSquaresInRow(_square, ItemsTypes.HORIZONTAL_STRIPED);
                        var list2 = GetSquaresInRow(_square, ItemsTypes.VERTICAL_STRIPED);
                        squares = squares.Union(list1.Union(list2).ToList()).ToList();
                        if (direction > 0)
                            MLevelManager.THIS.StripedShow(_square.gameObject, true);
                        else
                            MLevelManager.THIS.StripedShow(_square.gameObject, false);
                        direction *= -1;
                    }
                }
                if(package.square.type == SquareTypes.JellyBlock || striped.square.type == SquareTypes.JellyBlock)
                    MLevelManager.THIS.mLevelData.GetTargetObject().CheckSquares(squares.ToArray());
                var squaresToDestroy = squares.Distinct();
                var destroyingItems = squaresToDestroy.Where(i => i.MItem != null ).Select(i => i.MItem);
                item1.destroying = true;
                item2.destroying = true;
                yield return new WaitForSeconds(0.1f);
                foreach (var item in destroyingItems)
                {
                    item.destroyNext = true;
                    item.DestroyItem(true, this, this);
                }
                squaresToDestroy.Where(i=>i.MItem == null /*&& i.IsObstacle()*/).ForEachY(i => i.DestroyBlock());
                SoundBase.Instance.PlayLimitSound( SoundBase.Instance.explosion2 );
                striped.square.DestroyBlock();
                package.square.DestroyBlock();
                MLevelManager.THIS.mLevelData.GetTargetObject().CheckItems(list);
                striped.DestroyBehaviour();
                package.DestroyBehaviour();
                MLevelManager.THIS.FindMatches();
            }
        }


        /// <summary>
        /// is both striped items switched
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        private void CheckStripes(MItem item1, MItem item2)
        {
            var list = new[] { item1, item2 };
            var stripes = list.Where(i =>
                i.currentType == ItemsTypes.HORIZONTAL_STRIPED || i.currentType == ItemsTypes.VERTICAL_STRIPED);
            stripes = stripes.OrderBy(i => i == GetMItem).ToList();
            if (stripes.Count() < 2) return;
            item1.destroying = true;
            item2.destroying = true;
            item2?.DestroyBehaviour();
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.strippedExplosion);
            MLevelManager.THIS.StripedShow(gameObject, false);
            MLevelManager.THIS.StripedShow(gameObject, true);
            var list1 = GetSquaresInRow(GetMItem.square, ItemsTypes.HORIZONTAL_STRIPED);
            var list2 = GetSquaresInRow(GetMItem.square, ItemsTypes.VERTICAL_STRIPED);
            var lDistinct = list1.Union(list2).Distinct();
            if(square.type == SquareTypes.JellyBlock)
                MLevelManager.THIS.mLevelData.GetTargetObject().CheckSquares(lDistinct.ToArray());

            foreach (var square in lDistinct)
            {
                if (square.MItem != null ) square.MItem.DestroyItem(true,this,this);
                else /*if(square.IsObstacle())*/ square.DestroyBlock();
            }
            MLevelManager.THIS.mLevelData.GetTargetObject().CheckItems(list);
            item1.square.DestroyBlock();
            item1?.DestroyBehaviour();
        }

        public GameObject GetGameobject()
        {
            return gameObject;
        }

        private IEnumerator Timer(float sec, Action callback)
        {
            yield return new WaitForSeconds(sec);
            callback();
        }

        public bool IsCombinable()
        {
            return Combinable;
        }

        public bool IsExplodable()
        {
            return ActivateByExplosion;
        }

        public void SetExplodable(bool setExplodable)
        {
            ActivateByExplosion = setExplodable;
        }

        public bool IsStaticOnStart()
        {
            return StaticOnStart;
        }

        public void SetOrder(int i)
        {
            var spriteRenderers = GetSpriteRenderers();
            var orderedEnumerable = spriteRenderers.OrderBy(x => x.sortingOrder).ToArray();
            for (int index = 0; index < orderedEnumerable.Length; index++)
            {
                var spr = orderedEnumerable[index];
                spr.sortingOrder = i + index;
            }
        }

        public void SecondPartDestroyAnimation(Action callback)
        {
            throw new NotImplementedException();
        }
    }
}