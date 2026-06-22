using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawInspectorButtons();
    }

    private void DrawInspectorButtons()
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        bool drewHeader = false;

        foreach (MethodInfo method in methods)
        {
            InspectorButtonAttribute buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();
            if (buttonAttribute == null)
                continue;

            if (method.GetParameters().Length > 0)
            {
                DrawUnsupportedMethod(method, ref drewHeader);
                continue;
            }

            if (!drewHeader)
            {
                EditorGUILayout.Space();
                drewHeader = true;
            }

            string label = string.IsNullOrEmpty(buttonAttribute.Label) ? ObjectNames.NicifyVariableName(method.Name) : buttonAttribute.Label;
            if (GUILayout.Button(label))
                InvokeMethodForSelectedObjects(method.Name);
        }
    }

    private void DrawUnsupportedMethod(MethodInfo method, ref bool drewHeader)
    {
        if (!drewHeader)
        {
            EditorGUILayout.Space();
            drewHeader = true;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            GUILayout.Button($"{method.Name} needs parameters");
        }
    }

    private void InvokeMethodForSelectedObjects(string methodName)
    {
        foreach (UnityEngine.Object selectedTarget in targets)
        {
            MethodInfo method = selectedTarget.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
                continue;

            try
            {
                method.Invoke(selectedTarget, null);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, selectedTarget);
            }
        }
    }
}
