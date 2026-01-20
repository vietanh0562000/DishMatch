using System;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Item package
    /// </summary>
    public class MItemPackage : MItem, IItemInterface//, ILongDestroyable
    {
        // public bool Combinable;
        public bool ActivateByExplosion;
        public bool StaticOnStart;

        public GameObject explosion;
        public GameObject explosion2;
        public GameObject circle;

        private Action callbackDestroy;
        // private bool animationFinished;
        private int priority = 0;
        private bool canBeStarted;

        private MItem GetMItem
        {
            get
            {
                return GetComponentInParent<MItem>();
            }
        }

        public void Destroy(MItem item1, MItem item2)
        {
            if (GetMItem.square.type == SquareTypes.WireBlock)
            {
                GetMItem.square.DestroyBlock();
                GetMItem.StopDestroy();

                return;
            }

            gameObject.AddComponent<MGameBlocker>();
            item1.destroying = true;
            if (MLevelManager.THIS.DebugSettings.DestroyLog)
                DebugLogKeeper.Log(" pre destroy " + " type " + GetMItem.currentType + GetMItem.GetInstanceID() + " " + item1?.GetInstanceID() + " : " + item2?.GetInstanceID(), DebugLogKeeper.LogType.Destroying);
            GetParentItem().GetComponent<ItemDestroyAnimation>().DestroyPackage(item1);
        }


        public MItem GetParentItem()
        {
            return transform.GetComponentInParent<MItem>();
        }

        public override void Check(MItem item1, MItem item2)
        {
            if ((item2.currentType == ItemsTypes.MULTICOLOR))
            {
                item2.Check(item2, item1);
            }

            if ((item2.currentType == ItemsTypes.HORIZONTAL_STRIPED || item2.currentType == ItemsTypes.VERTICAL_STRIPED))
            {
                item2.Check(item2, item1);
            }

            if (item2.currentType == ItemsTypes.MARMALADE)
            {
                item2.Check(item2, item1);
            }

            if (item1.currentType == ItemsTypes.PACKAGE && item2.currentType == ItemsTypes.PACKAGE)
            {
                item1.GetTopItemInterface().Destroy(item1, item2);
                item2.GetTopItemInterface().Destroy(item2, item1);
            }
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

        public bool IsAnimationFinished()
        {
            return animationFinished;
        }

        public int GetPriority()
        {
            return priority;
        }

        public bool CanBeStarted()
        {
            return canBeStarted;
        }
    }
}
