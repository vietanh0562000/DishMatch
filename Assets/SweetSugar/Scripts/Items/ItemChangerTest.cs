
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Debug item menu by right mouse button. Works only in Unity editor
    /// </summary>
    public class ItemChangerTest : MonoBehaviour
    {
        private MItem _mItem;
        public void ShowMenuItems(MItem mItem)
        {
            _mItem = mItem;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (_mItem != null)
            {
                if (GUILayout.Button("Select item"))
                {
                    Selection.objects = new Object[] { _mItem.gameObject };
                }       
//            if (GUILayout.Button("Show in console"))
//            {
//                Debug.Log("\nCPAPI:{\"cmd\":\"Search\" \"text\":\"" + item.instanceID + "\"}");
//            }
                if (GUILayout.Button("Select square"))
                {
                    Selection.objects = new Object[] { _mItem.square.gameObject };
                }
                foreach (var itemType in EnumUtil.GetValues<ItemsTypes>())
                {
                    if (GUILayout.Button(itemType.ToString()))
                    {
                        _mItem.SetType(itemType);
                        _mItem = null;
                        // item.debugType = itemType;
                    }
                }
                if (GUILayout.Button("Destroy"))
                {
                    _mItem.DestroyItem(true);
                    MLevelManager.THIS.FindMatches();
                    _mItem = null;
                }
            }
        }
#endif
    }
}