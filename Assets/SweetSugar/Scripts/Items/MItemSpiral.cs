using System;
using System.Linq;
using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Spiral item
    /// </summary>
    public class MItemSpiral:MItem,IItemInterface
    {
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        public void Destroy(MItem item1, MItem item2)
        {
            item1.DestroyBehaviour();
        }

        public GameObject GetGameobject()
        {
            return gameObject;
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
            if (MLevelManager.THIS.gameStatus != GameState.Playing)
                return StaticOnStart;
            return false;
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

        public MItem GetParentItem()
        {
            return this;
        }

        public void SecondPartDestroyAnimation(Action callback)
        {
            throw new NotImplementedException();
        }
    }
}