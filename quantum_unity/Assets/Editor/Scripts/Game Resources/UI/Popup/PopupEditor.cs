using UnityEditor;
using UnityEngine;

using GRPopup = GameResources.UI.Popup.Popup;

namespace Editor.GameResources.UI.Popup
{
    [CustomEditor(typeof(GRPopup), true)]
    [CanEditMultipleObjects]
    internal class PopupEditor : UnityEditor.Editor
    {
        private static int _toolbarIndex = 0;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space(2);

            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, new GUIContent[] { new("Info"), new("Events"), new("Input") });
            EditorGUILayout.Space(5);

            switch (_toolbarIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_title"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_description"));
                    break;

                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_responses"));
                    break;

                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_inputResponse"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
