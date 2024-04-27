using UnityEditor;
using UnityEngine;
using Extensions.Types;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Array2D<>))]
    internal class Array2DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PrefixLabel(position, label);
            position.position += new Vector2(0, EditorGUIUtility.singleLineHeight);

            SerializedProperty array = property.FindPropertyRelative("_array");
            for (int i = 0; i < array.arraySize; ++i)
            {
                SerializedProperty array2 = array.GetArrayElementAtIndex(i).FindPropertyRelative("_array");
                for (int j = 0; j < array2.arraySize; ++j)
                {
                    Rect rect = position;
                    rect.width = EditorGUIUtility.singleLineHeight * 4;
                    rect.position += new Vector2(rect.width * i * 0.4f, rect.height * j);

                    EditorGUI.PropertyField(rect, array2.GetArrayElementAtIndex(j), GUIContent.none);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty array = property.FindPropertyRelative("_array").GetArrayElementAtIndex(0).FindPropertyRelative("_array");
            return base.GetPropertyHeight(property, label) * (array.arraySize + 1) + 8;
        }
    }
}
