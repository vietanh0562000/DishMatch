// https://github.com/Unity-Technologies/UnityCsReference (Editor/Mono/GameView)
// https://answers.unity.com/questions/956123/add-and-select-game-view-resolution.html
// https://answers.unity.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html

#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace TRS.CaptureTool
{
    public static class GameView
    {
        const string TEMP_SIZE_PREFIX = "Temp: ";
#if UNITY_2018_1_OR_NEWER
        readonly static GameViewSizeGroupType[] gameViewSizeGroupTypes = { GameViewSizeGroupType.Android, GameViewSizeGroupType.HMD, GameViewSizeGroupType.iOS, GameViewSizeGroupType.Standalone };
#elif UNITY_2017_1_OR_NEWER
        readonly static GameViewSizeGroupType[] gameViewSizeGroupTypes = { GameViewSizeGroupType.Android, GameViewSizeGroupType.HMD, GameViewSizeGroupType.iOS, GameViewSizeGroupType.N3DS, GameViewSizeGroupType.Standalone };
#else
        readonly static GameViewSizeGroupType[] gameViewSizeGroupTypes = { GameViewSizeGroupType.Android, GameViewSizeGroupType.iOS, GameViewSizeGroupType.N3DS, GameViewSizeGroupType.Standalone };
#endif

        public enum GameViewSizeType
        {
            AspectRatio, FixedResolution, Size
        }

        static object gameViewSizesInstance;
        static MethodInfo getGroup;

        static GameView()
        {
            // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
            var gameViewSizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = gameViewSizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }

        [MenuItem("Tools/Additional Resolutions/Remove Temp Resolutions")]
        public static void RemoveTempSizes()
        {
            foreach (GameViewSizeGroupType gameViewSizeGroupType in gameViewSizeGroupTypes)
            {
                string[] sizeNames = SizeNames(gameViewSizeGroupType);
                for (int i = sizeNames.Length - 1; i >= 0; i--)
                {
                    if (IsCustomSize(gameViewSizeGroupType, i) && sizeNames[i].Contains(TEMP_SIZE_PREFIX))
                        RemoveCustomSize(gameViewSizeGroupType, i);
                }
            }
        }

        // Example of how to add buttons to add resolutions as selectable in GameView
        [MenuItem("Tools/Additional Resolutions/Add Android Sizes", false, 300)]
        public static void AddAndroidSizes()
        {
            AddSizesForGroupType(GameViewSizeGroupType.Android);
        }

        [MenuItem("Tools/Additional Resolutions/Add iOS Sizes", false, 301)]
        public static void AddiOSSizes()
        {
            AddSizesForGroupType(GameViewSizeGroupType.iOS);
        }

        [MenuItem("Tools/Additional Resolutions/Add Promotional Sizes", false, 302)]
        public static void AddPromotionalSizes()
        {
            AddSizesFromDictionary(AdditionalResolutions.promotional, GameViewSizeGroupType.Standalone, "Promotional");
        }

        [MenuItem("Tools/Additional Resolutions/Remove All Custom Sizes")]
        public static void RemoveAllCustomSizes()
        {
            foreach (GameViewSizeGroupType gameViewSizeGroupType in gameViewSizeGroupTypes)
            {
                string[] sizeNames = SizeNames(gameViewSizeGroupType);
                for (int i = sizeNames.Length - 1; i >= 0; i--)
                {
                    // Wrap in an if statement to only clear certain types
                    // if (sizeNames[i].Contains("Asset Store"))
                    // {
                    if (IsCustomSize(gameViewSizeGroupType, i))
                        RemoveCustomSize(gameViewSizeGroupType, i);
                    // }
                }
            }
        }

        static void AddSizesForGroupType(GameViewSizeGroupType groupType)
        {
            Dictionary<string, Resolution> sizes = AdditionalResolutions.forGroupType[groupType];
            AddSizesFromDictionary(sizes, groupType);
        }

        static void AddSizesFromDictionary(Dictionary<string, Resolution> sizes, GameViewSizeGroupType groupType, string sizeGroupName = "")
        {
            Dictionary<string, Resolution> allSizes = AllSizes();

            foreach (string key in sizes.Keys)
            {
                string name = AdditionalResolutions.ConvertToGameViewSizeName(key);
                if (!allSizes.ContainsKey(name))
                {
                    Resolution resolution = sizes[key];
                    AddCustomSize(groupType, GameViewSizeType.FixedResolution, resolution.width, resolution.height, name);
                }
            }

            if (sizeGroupName == null || sizeGroupName.Length <= 0)
                sizeGroupName = groupType.ToString();

            Debug.Log(sizeGroupName + " Sizes added to " + groupType + " sizes group (only available when buid platform is " + groupType + ").");
        }

        public static Vector2 CurrentSize()
        {
            System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            MethodInfo GetSizeOfMainGameView = type.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            System.Object result = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)result;
        }

        public static GameViewSizeType CurrentGameViewSizeType()
        {
            int selectedSizeIndex = GetSelectedSizeIndex();
            var group = GetGroup(GetCurrentGroupType());
            var groupType = group.GetType();

            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gameViewSizeType = getGameViewSize.ReturnType;
            var sizeTypeProp = gameViewSizeType.GetProperty("sizeType");

            var indexValue = new object[1];
            indexValue[0] = selectedSizeIndex;
            var size = getGameViewSize.Invoke(group, indexValue);
            return (GameViewSizeType)(int)sizeTypeProp.GetValue(size, null);
        }

        public static Dictionary<string, Resolution> AllSizes(bool divided = false)
        {
            return AllSizes(GetCurrentGroupType(), divided);
        }

        public static Dictionary<string, Resolution> AllSizes(GameViewSizeGroupType sizeGroupType, bool divided = false)
        {
            Dictionary<string, Resolution> allSizes = new Dictionary<string, Resolution>();

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();

            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gameViewSizeType = getGameViewSize.ReturnType;
            var widthProp = gameViewSizeType.GetProperty("width");
            var heightProp = gameViewSizeType.GetProperty("height");

            var indexValue = new object[1];
            string[] sizeNames = SizeNames(sizeGroupType);
            for (int i = 0; i < sizeNames.Length; i++)
            {
                string sizeName = TrimmedSizeName(sizeNames[i]);
                if (divided)
                {
                    if (IsCustomSize(sizeGroupType, i))
                        sizeName = "Custom/" + sizeName;
                    else
                        sizeName = "Default/" + sizeName;
                }

                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);

                allSizes[sizeName] = new Resolution { width = sizeWidth, height = sizeHeight };
            }

            return allSizes;
        }

        public static string NameForSize(Resolution resolution)
        {
            return NameForSize(resolution.width, resolution.height);
        }

        public static string NameForSize(int width, int height)
        {
            int sizeIndex = FindSize(width, height);
            if (sizeIndex < 0)
                return "";

            string[] sizeNames = SizeNames();
            return TrimmedSizeName(sizeNames[sizeIndex]);
        }

        #region SizeExists Overloads
        public static bool SizeExists(string text)
        {
            return SizeExists(GetCurrentGroupType(), text);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
        {
            return FindSize(sizeGroupType, text) != -1;
        }

        public static bool SizeExists(Resolution resolution)
        {
            return SizeExists(resolution.width, resolution.height);
        }

        public static bool SizeExists(int width, int height)
        {
            return SizeExists(GetCurrentGroupType(), width, height);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, Resolution resolution)
        {
            return SizeExists(sizeGroupType, resolution.width, resolution.height);
        }
        #endregion

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }

        public static int FindSize(string text)
        {
            return FindSize(GetCurrentGroupType(), text);
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            string[] sizeNames = SizeNames(sizeGroupType);
            for (int i = 0; i < sizeNames.Length; i++)
            {
                string sizeName = TrimmedSizeName(sizeNames[i]);
                if (sizeName == text)
                    return i;
            }
            return -1;
        }

        #region FindSize Overloads
        public static int FindSize(Resolution resolution)
        {
            return FindSize(resolution.width, resolution.height);
        }

        public static int FindSize(int width, int height)
        {
            return FindSize(GetCurrentGroupType(), width, height);
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, Resolution resolution)
        {
            return FindSize(sizeGroupType, resolution.width, resolution.height);
        }
        #endregion

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            // goal:
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }
            return -1;
        }

        #region SetSize Overloads
        public static void SetSize(Resolution resolution)
        {
            SetSize(GameViewSizeType.FixedResolution, resolution.width, resolution.height);
        }

        public static void SetSize(int width, int height)
        {
            SetSize(GameViewSizeType.FixedResolution, width, height);
        }

        public static void SetSize(GameViewSizeType gameViewSizeType, Resolution resolution)
        {
            SetSize(gameViewSizeType, resolution.width, resolution.height);
        }
        #endregion

        public static void SetSize(GameViewSizeType gameViewSizeType, int width, int height)
        {
            GameViewSizeGroupType gameViewSizeGroupType = GetCurrentGroupType();
            int index = FindSize(gameViewSizeGroupType, width, height);
            if (index != -1)
                SetSelecedSizeIndex(index);
            else
            {
                // The below line is necessary or Unity will crash
                AddCustomSize(gameViewSizeGroupType, gameViewSizeType, width, height, TempSizeName(width, height));
                SetSize(gameViewSizeType, width, height);
            }
        }

        public static int GetSelectedSizeIndex()
        {
            var gameViewWindowType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndex = gameViewWindowType.GetMethod("get_selectedSizeIndex",
                                                                 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
            var gameViewWindow = EditorWindow.GetWindow(gameViewWindowType);
            return (int)selectedSizeIndex.Invoke(gameViewWindow, null);
        }

        public static void SetSelecedSizeIndex(int index)
        {
            var gameViewWindowType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var sizeSelectionCallback = gameViewWindowType.GetMethod("SizeSelectionCallback",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var gameViewWindow = EditorWindow.GetWindow(gameViewWindowType);
            sizeSelectionCallback.Invoke(gameViewWindow, new object[] { index, null });
        }

        #region AddTempCustomSize Overloads
        public static void AddTempCustomSize(Resolution resolution)
        {
            AddTempCustomSize(GameViewSizeType.FixedResolution, resolution);
        }

        public static void AddTempCustomSize(int width, int height)
        {
            AddTempCustomSize(GetCurrentGroupType(), GameViewSizeType.FixedResolution, width, height);
        }

        public static void AddTempCustomSize(GameViewSizeType viewSizeType, Resolution resolution)
        {
            AddTempCustomSize(GetCurrentGroupType(), viewSizeType, resolution);
        }

        public static void AddTempCustomSize(GameViewSizeType viewSizeType, int width, int height)
        {
            AddTempCustomSize(GetCurrentGroupType(), viewSizeType, width, height);
        }

        public static void AddTempCustomSize(GameViewSizeGroupType sizeGroupType, GameViewSizeType viewSizeType, Resolution resolution)
        {
            AddTempCustomSize(sizeGroupType, viewSizeType, resolution.width, resolution.height);
        }
        #endregion

        public static void AddTempCustomSize(GameViewSizeGroupType sizeGroupType, GameViewSizeType viewSizeType, int width, int height)
        {
            AddCustomSize(sizeGroupType, viewSizeType, width, height, TempSizeName(width, height));
        }

        #region AddCustomSize Overloads
        public static void AddCustomSize(Resolution resolution, string text)
        {
            AddCustomSize(GetCurrentGroupType(), GameViewSizeType.FixedResolution, resolution, text);
        }

        public static void AddCustomSize(int width, int height, string text)
        {
            AddCustomSize(GetCurrentGroupType(), GameViewSizeType.FixedResolution, width, height, text);
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, Resolution resolution, string text)
        {
            AddCustomSize(GetCurrentGroupType(), viewSizeType, resolution, text);
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, int width, int height, string text)
        {
            AddCustomSize(GetCurrentGroupType(), viewSizeType, width, height, text);
        }

        public static void AddCustomSize(GameViewSizeGroupType sizeGroupType, GameViewSizeType viewSizeType, Resolution resolution, string text)
        {
            AddCustomSize(sizeGroupType, viewSizeType, resolution.width, resolution.height, text);
        }
        #endregion

        public static void AddCustomSize(GameViewSizeGroupType sizeGroupType, GameViewSizeType viewSizeType, int width, int height, string text)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
            // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);


            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new System.Type[] { typeof(GameViewSizeType), typeof(int), typeof(int), typeof(string) });
            if (ctor != null)
            {
                var newSize = ctor.Invoke(new object[] { viewSizeType, width, height, text });
                addCustomSize.Invoke(group, new object[] { newSize });
                return;
            }
            var allConstructors = gvsType.GetConstructors();
            var altNewSize = allConstructors[0].Invoke(new object[] { (int)viewSizeType, width, height, text });
            if (altNewSize != null)
            {
                addCustomSize.Invoke(group, new object[] { altNewSize });
                return;
            }

            Debug.LogError("Failed to create game view size: " + text + " (" + width + "x" + height + ")\n\nManually create size to continue. Email: jacob@tangledrealitystudios.com and I'll create an update to fix it.");
        }

        public static bool IsCustomSize(int index)
        {
            return IsCustomSize(GetCurrentGroupType(), index);
        }

        public static bool IsCustomSize(GameViewSizeGroupType sizeGroupType, int index)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
            // group.RemoveCustomSize(index);

            var group = GetGroup(sizeGroupType);
            var isCustomSize = getGroup.ReturnType.GetMethod("IsCustomSize");
            return (bool)isCustomSize.Invoke(group, new object[] { index });
        }

        public static void RemoveCustomSize(int index)
        {
            RemoveCustomSize(GetCurrentGroupType(), index);
        }

        public static void RemoveCustomSize(GameViewSizeGroupType sizeGroupType, int index)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
            // group.RemoveCustomSize(index);

            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("RemoveCustomSize");
            addCustomSize.Invoke(group, new object[] { index });
        }

        public static GameViewSizeGroupType GetCurrentGroupType()
        {
            var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
            return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
        }


        #region Helpers
        static string TempSizeName(Resolution resolution)
        {
            return TempSizeName(resolution.width, resolution.height);
        }

        static string TempSizeName(int width, int height)
        {
            return TEMP_SIZE_PREFIX + width + "x" + height;
        }

        static string TrimmedSizeName(string sizeName)
        {
            // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
            // so if we're querying a custom size text we substring to only get the name
            // You could see the outputs by just logging
            int pren = sizeName.LastIndexOf('(');
            if (pren > 0)
                sizeName = sizeName.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
            return sizeName;
        }

        static string[] SizeNames()
        {
            return SizeNames(GetCurrentGroupType());
        }

        static string[] SizeNames(GameViewSizeGroupType sizeGroupType)
        {
            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            return displayTexts;
        }

        static object GetGroup(GameViewSizeGroupType sizeGroupType)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)sizeGroupType });
        }
        #endregion
    }
}
#else
        public static class GameView { }
#endif