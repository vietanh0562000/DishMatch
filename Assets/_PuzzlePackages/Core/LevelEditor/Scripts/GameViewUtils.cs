using System;
using System.Reflection;

namespace BasePuzzle.PuzzlePackages.Core
{
    using UnityEngine;

    public static class GameViewUtils
    {
#if UNITY_EDITOR

        private static object GetGameViewSizesInstance()
        {
            var sizesType    = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType   = typeof(UnityEditor.ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            return instanceProp != null ? instanceProp.GetValue(null, null) : null;
        }

        private static object GetGroup(UnityEditor.GameViewSizeGroupType type)
        {
            var gameViewSizesInstance = GetGameViewSizesInstance();
            var getGroup = gameViewSizesInstance.GetType().GetMethod("GetGroup");
            return getGroup != null ? getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type }) : null;
        }

        private static void AddCustomSize(int width, int height, UnityEditor.GameViewSizeGroupType groupType, string text)
        {
            var group = GetGroup(groupType);
            var addCustomSize = group.GetType().GetMethod("AddCustomSize");
            var assemblyName = "UnityEditor.dll";
            
            Assembly assembly = Assembly.Load(assemblyName);
            Type gameViewSize = assembly.GetType("UnityEditor.GameViewSize");
            Type gameViewSizeType = assembly.GetType("UnityEditor.GameViewSizeType"); // Lấy Type
            
            ConstructorInfo ctor = gameViewSize.GetConstructor(new Type[]
            {
                gameViewSizeType,
                typeof(int),
                typeof(int),
                typeof(string)
            });

            if (ctor == null) return;
            var newSize = ctor.Invoke(new object[] { 1, width, height, text }); // 1 = Fixed Resolution
            if (addCustomSize != null) addCustomSize.Invoke(group, new object[] { newSize });
        }

        private static int FindSize(UnityEditor.GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            
            if (getBuiltinCount == null) return -1;
            if (getCustomCount == null) return -1;
            
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            if (getGameViewSize == null) return -1;
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                if (widthProp == null) continue;
                int sizeWidth = (int)widthProp.GetValue(size, null);
                if (heightProp == null) continue;
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }

            return -1;
        }

        private static void SetSize(int index)
        {
            var gvWndType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = UnityEditor.EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp?.SetValue(gvWnd, index, null);
        }

        public static void SetGameViewSize(int width, int height, UnityEditor.GameViewSizeGroupType type)
        {
            UnityEditor.GameViewSizeGroupType groupType = type; // Mặc định là Standalone

            // 1. Tìm kích thước
            int index = FindSize(groupType, width, height);

            // 2. Nếu không tìm thấy, thêm kích thước mới
            if (index == -1)
            {
                AddCustomSize(width, height, groupType, $"{width}x{height}");
                index = FindSize(groupType, width, height); // Tìm lại sau khi thêm
            }

            // 3. Đặt kích thước Game View
            if (index != -1)
            {
                SetSize(index);
            }
        }

        public static void SetFullScreen()
        {
            SetGameViewSize(2080, 1920, UnityEditor.GameViewSizeGroupType.Android);
        }
#endif
    }
}