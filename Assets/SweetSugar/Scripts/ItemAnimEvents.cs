using SweetSugar.Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace SweetSugar.Scripts
{
    public class ItemAnimEvents : MonoBehaviour {


        [FormerlySerializedAs("item")] public MItem mItem;

        public void SetAnimationDestroyingFinished()
        {
            mItem.SetAnimationDestroyingFinished();
        }
    }
}
