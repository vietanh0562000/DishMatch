using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    [CustomEditor(typeof(MonoCustomInspector), true)]
    public class InspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var mono = target as MonoBehaviour;
            if (mono == null) return;
            
            var drawAction = new List<Action>();

            var script = mono.GetType().GetCustomAttribute<HideComponentFieldAttribute>();
            if (script == null)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            // Lấy danh sách fields theo thứ tự khai báo
            var fields = mono.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Lấy danh sách methods có MyButton attribute theo thứ tự khai báo
            var methods = mono.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(m => new
                {
                    Method = m,
                    Attribute = m.GetCustomAttribute<InspectorButtonAttribute>(),
                })
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.order)
                .ToList();

            //Thêm action draw field lên inspector vào list
            foreach (var f in fields)
            {
                if (f.GetCustomAttribute<HideInInspector>() != null) continue;
                
                drawAction.Add(() =>
                {
                    var prop = serializedObject.FindProperty(f.Name);
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop);
                    }
                });
            }

            //Thêm action draw method lên inspector vào list
            foreach (var m in methods)
            {
                if (m.Attribute.order < 0)
                {
                    Debug.LogError("Attribute order must be greater than 0");
                    continue;
                }

                if (m.Attribute.order < drawAction.Count)
                {
                    drawAction.Insert(m.Attribute.order, () => DrawButtonMethod(m.Method, m.Attribute, mono));
                    continue;
                }

                drawAction.Add(() => { DrawButtonMethod(m.Method, m.Attribute, mono); });
            }

            //Gọi action để draw fields và methods lên inspector
            foreach (var action in drawAction)
            {
                action.Invoke();
            }

            serializedObject.ApplyModifiedProperties();
            
        }

        private void DrawButtonMethod(MethodInfo method, InspectorButtonAttribute attribute, MonoBehaviour mono)
        {
            if (attribute.marginTop != 0)
                EditorGUILayout.Space(attribute.marginTop);

            if (GUILayout.Button(attribute.methodName))
            {
                method.Invoke(mono, null);
            }

            if (attribute.marginBot != 0)
                EditorGUILayout.Space(attribute.marginBot);
        }
    }
}