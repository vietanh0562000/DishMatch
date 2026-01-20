#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EffectToolEditorNonSpine : EditorWindow
{
    [MenuItem("MyTool/Effect Tool")]
    static void ShowWindow()
    {
        var editor = GetWindow(typeof(EffectToolEditorNonSpine));
        editor.Show();
    }

    public int CurrentIndexLevel;

    void OnGUI()
    {
        GUILayout.Label("EFFECT");
        Time.timeScale = EditorGUILayout.Slider("Time Scale", Time.timeScale, 0, 10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play Particle"))
            PlayParticle();
        if (GUILayout.Button("Play Effect"))
            PlayEffect();
        if (GUILayout.Button("Apply Prefab"))
            PlayEffect();
        EditorGUILayout.EndHorizontal();
       
      
      
    }

    [MenuItem("MyTool/EffectTool/PlayParticle &a")]
    public static void PlayParticle()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj != null)
        {
            var particle = obj.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Clear();
                particle.Play();
            }
        }
    }
    [MenuItem("MyTool/EffectTool/PlayEffect &s")]
    public static void PlayEffect()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj != null)
        {
            var particle = obj.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                var rootObj = GetRoot(particle.gameObject);
                var rootParticle = rootObj.GetComponent<ParticleSystem>();
                rootParticle.Clear();
                rootParticle.Play();
            }
        }
    }

    private static GameObject GetRoot(GameObject obj)
    {
        var parent = obj.transform.parent;
        if (parent != null)
        {
            var particle = parent.GetComponent<ParticleSystem>();
            if (particle != null)
                return GetRoot(particle.gameObject);
        }
        return obj;
    }

    [MenuItem("MyTool/Apply All Prefab")]
    public static void Apply()
    {
        var list = Selection.gameObjects;
        if (list.Length > 0)
            foreach (var obj in list)
                PrefabUtility.ReplacePrefab(PrefabUtility.FindPrefabRoot(obj), PrefabUtility.GetPrefabParent(obj), ReplacePrefabOptions.ConnectToPrefab);
    }
}
#endif