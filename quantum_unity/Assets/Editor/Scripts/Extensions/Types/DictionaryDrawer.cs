using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Extensions.Types;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Dictionary<,>))]
    internal class DictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty kvpsProperty = property.FindPropertyRelative("_kvps");
            ReorderableList list = new(property.serializedObject, kvpsProperty, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => DrawHeader(rect, property.displayName),
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => DrawElement(rect, kvpsProperty.GetArrayElementAtIndex(index)),
                elementHeightCallback = (int index) => ElementHeight(kvpsProperty.GetArrayElementAtIndex(index)),
                onAddCallback = (ReorderableList list) => OnAdd(kvpsProperty)
            };

            list.DoList(position);

            EditorGUI.EndProperty();
        }
        
        private void DrawHeader(Rect rect, string label)
        {
            GUI.Label(rect, label);
        }

        private void DrawElement(Rect rect, SerializedProperty kvpsProperty)
        {
            EditorGUI.PropertyField(new Rect(rect.position, new Vector2(rect.width, EditorGUIUtility.singleLineHeight)), kvpsProperty);
        }
        
        private float ElementHeight(SerializedProperty kvpsProperty)
        {
            return EditorGUI.GetPropertyHeight(kvpsProperty) + 2;
        }

        private void OnAdd(SerializedProperty kvpsProperty)
        {
            kvpsProperty.InsertArrayElementAtIndex(kvpsProperty.arraySize);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty kvpsProperty = property.FindPropertyRelative("_kvps");

            float size = 0;
            if (kvpsProperty.arraySize > 0)
            {
                for (int i = 0; i < kvpsProperty.arraySize; ++i)
                {
                    size += EditorGUI.GetPropertyHeight(kvpsProperty.GetArrayElementAtIndex(i)) + 4;
                }
            }
            else
            {
                size = EditorGUIUtility.singleLineHeight;
            }

            return size + EditorGUIUtility.singleLineHeight * 5f;
        }
    }
}
