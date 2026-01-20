using System;
using System.Linq;
using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Simple item
    /// </summary>
    public class MItemSimple : MItem, IItemInterface
    {
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        public void Destroy(MItem item1, MItem item2)
        {
            GetParentItem().square.DestroyBlock();
            item1.DestroyBehaviour();
        }

        public override void Check(MItem item1, MItem item2)
        {
            if (item2.currentType != ItemsTypes.NONE)
                item2.Check(item2, item1);

        }

        public MItem GetParentItem()
        {
            return transform.GetComponentInParent<MItem>();
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

        public void SecondPartDestroyAnimation(Action callback)
        {
            throw new NotImplementedException();
        }
    }
}