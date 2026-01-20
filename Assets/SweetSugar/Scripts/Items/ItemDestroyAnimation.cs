using System.Collections;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Effects;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Package destroy animation helper
    /// </summary>
    public class ItemDestroyAnimation : MonoBehaviour
    {
        //    public PlayableDirector director;
        //    public PlayableAsset[] timelines;
        private MItem _mItem;
        private bool started;

        private void Start()
        {
            _mItem = GetComponent<MItem>();
        }

        public void DestroyPackage(MItem item1)
        {
            if (started) return;
            started = true;
            var thisItem = GetComponent<MItem>();

            GameObject go = Instantiate(Resources.Load("Prefabs/Effects/_ExplosionAround") as GameObject);//LevelManager.THIS.GetExplAroundPool();
            if (go != null)
            {
                go.transform.position = transform.position;
                var explosionAround = go.GetComponent<ExplAround>();
                explosionAround.mItem = thisItem;
                go.SetActive(true);
            }

            var square = item1.square;
            square.MItem = item1;

//        item.anim.enabled = true;
//        var audioBinding = item.director.playableAsset.outputs.Select(i => i.sourceObject).OfType<AudioTrack>().FirstOrDefault();
//        item.director.SetGenericBinding(audioBinding, SoundBase.Instance);
//        item.director.Play();
            StartCoroutine(OnPackageAnimationFinished(item1));
        }

        private IEnumerator OnPackageAnimationFinished(MItem item1)
        {
            var square = item1.square;
//        yield return new WaitUntil(() => item.director.time >= .35f || item.director.state == PlayState.Paused);
            yield return new WaitForSeconds(.35f);
            DestroyItems(item1, square);
            yield return new WaitForSeconds(0.2f);
            _mItem.HideSprites(true);
//        yield return new WaitUntil(() => item.director.time >= item.director.duration || item.director.state == PlayState.Paused);
            yield return new WaitForSeconds(0.5f);
            _mItem.DestroyBehaviour();
            started = false;

        }

        private void DestroyItems(MItem item1, Square square)
        {
            MLevelManager.THIS.field.DestroyItemsAround(square, _mItem);
            var sqList = MLevelManager.THIS.GetSquaresAroundSquare(square);
            square.DestroyBlock();
            if(square.type == SquareTypes.JellyBlock)
                MLevelManager.THIS.mLevelData.GetTargetObject().CheckSquares(sqList.ToArray());
            item1.destroying = false;
            _mItem.square.MItem = null;
        }
    }
}

