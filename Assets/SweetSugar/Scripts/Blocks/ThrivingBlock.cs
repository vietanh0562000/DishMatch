using System.Collections;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts.Blocks
{
    /// <summary>
    /// block expands constantly until you explode one
    /// </summary>
    public class ThrivingBlock : Square
    {
        static bool blockCreated;
        int lastMoveID = -1;

        void OnEnable()
        {
            MLevelManager.OnTurnEnd += OnTurnEnd;
        }

        void OnDisable()
        {
            MLevelManager.OnTurnEnd -= OnTurnEnd;
            MLevelManager.THIS.thrivingBlockDestroyed = true;
        }

        private void OnTurnEnd()
        {
            if (MLevelManager.THIS.moveID == lastMoveID) return;
            lastMoveID = MLevelManager.THIS.moveID;
            if (MLevelManager.THIS.thrivingBlockDestroyed || blockCreated || field != MLevelManager.THIS.field) return;
            MLevelManager.THIS.thrivingBlockDestroyed = false;
            var sqList = this.mainSquare.GetAllNeghborsCross();
            foreach (var sq in sqList)
            {
                if (!sq.CanGoInto() || Random.Range(0, 1) != 0 ||
                    sq.type != SquareTypes.EmptySquare || sq.MItem?.currentType != ItemsTypes.NONE) continue;
                if (sq.MItem == null) continue;
                if (sq.MItem.currentType != ItemsTypes.NONE) continue;
                sq.CreateObstacle(SquareTypes.ThrivingBlock, 1);
                blockCreated = true;
                StartCoroutine(blockCreatedCD());
                Destroy(sq.MItem.gameObject);
                break;
            }
        }

        IEnumerator blockCreatedCD()
        {
            yield return new WaitForSeconds(1);
            blockCreated = false;
        }
    }
}