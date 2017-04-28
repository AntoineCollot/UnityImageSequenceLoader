﻿// Initial Concept by http://www.reddit.com/user/zaikman
// Revised by http://www.reddit.com/user/quarkism

using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

/// <summary>
/// Stick this on a method
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class EditorButtonAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class EditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mono = target as MonoBehaviour;

        var methods = mono.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic)
            .Where(o => Attribute.IsDefined(o, typeof(EditorButtonAttribute)));

        foreach (var memberInfo in methods)
        {
            if (GUILayout.Button(memberInfo.Name))
            {
                var method = memberInfo as MethodInfo;
                method.Invoke(mono, null);
            }
        }
    }
}
#endif