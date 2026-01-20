using SweetSugar.Scripts.MapScripts.StaticMap.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SweetSugar.Scripts.Editor
{
    [InitializeOnLoad]
    public static class EditorMenu
    {
        private const string MenuMapStatic = "Sweet Sugar/Scenes/Map switcher/Static map";
        private const string MenuMapDinamic = "Sweet Sugar/Scenes/Map switcher/Dinamic map";

    [MenuItem("Sweet Sugar/Scenes/Main scene")]
    public static void MainScene()
    {
        EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/main.unity");
    }
    
    [MenuItem(MenuMapStatic)]
    public static void MapSceneStatic()
    {
        SetStaticMap( true);
        GameScene();
    }
    
    [MenuItem(MenuMapDinamic)]
    public static void MapSceneDinamic()
    {
        SetStaticMap( false);
        GameScene();
    }
    
    public static void SetStaticMap(bool enabled) {
 
        Menu.SetChecked(MenuMapStatic, enabled);
        Menu.SetChecked(MenuMapDinamic, !enabled);
        var sc = Resources.Load<MapSwitcher>("Scriptable/MapSwitcher");
        sc.staticMap = enabled;
        EditorUtility.SetDirty(sc);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Sweet Sugar/Scenes/Game scene")]
    public static void GameScene()
    {
        EditorSceneManager.OpenScene("Assets/SweetSugar/Scenes/"+Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName()+".unity");
    }

    [MenuItem("Sweet Sugar/Settings/Debug settings")]
    public static void DebugSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/SweetSugar/Resources/Scriptable/DebugSettings.asset");
    }
    [MenuItem("Sweet Sugar/Settings/Pool settings")]
    public static void PoolSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/SweetSugar/Resources/Scriptable/PoolSettings.asset");
    }
    //     [MenuItem("Sweet Sugar/procc")]
//     public static void Start()
//     {
//         var targets = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Resources/Levels/TargetEditorScriptable.asset", typeof(TargetEditorScriptable)) as TargetEditorScriptable;
//         var target = targets.targets.Where(i => i.name == "Ingredients").First();
//         var levelData = AssetDatabase.LoadAssetAtPath("Assets/SweetSugar/Resources/Levels/LevelScriptable.asset", typeof(LevelScriptable)) as LevelScriptable;
//         foreach (var level in levelData.levels)
//         {
//             if (level.target.name == "Ingredients")
//             {
//                 level.target = target.DeepCopy();
//                 Debug.Log(level.levelNum);
//             }
//         }
//     }   
    }
}