using UnityEditor;
using UnityEngine;
using Extensions.Components.Input;

namespace Editor.Extensions.Components.Input
{
    [CustomEditor(typeof(InputEvent), true)]
    [CanEditMultipleObjects]
    internal class InputEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_button"));
            if (!(target as InputEvent).Button)
            {
                EditorGUILayout.HelpBox("Assign an InputButton to begin creating events.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);
            }
            else if ((target as InputEvent).Button.Action is null)
            {
                EditorGUILayout.HelpBox("The given InputButton has no InputAction assigned to it.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_readContinuous"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_processWhenEmpty"));

            switch ((target as InputEvent).Type)
            {
                case "Axis":
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_onAxisAction"), new GUIContent("On Action"));
                    break;

                case "Vector2":
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_onVector2Action"), new GUIContent("On Action"));
                    break;

                default:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_onAction"), new GUIContent("On Action"));
                    break;
            }

            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
