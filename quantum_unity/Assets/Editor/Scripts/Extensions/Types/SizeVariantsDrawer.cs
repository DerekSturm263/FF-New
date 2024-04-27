using UnityEditor;
using UnityEngine;
using Extensions.Types;
using static Editor.Extensions.Miscellaneous.EditorHelper;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(SizeVariants<>))]
    internal class SizeVariantsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

            Rect indentedRect = new(position.position + new Vector2(10, EditorGUIUtility.singleLineHeight), position.size + new Vector2(-10, 0));

            DrawProperty(ref indentedRect, property, "_small");
            DrawProperty(ref indentedRect, property, "_medium");
            DrawProperty(ref indentedRect, property, "_large");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CombinePropertyHeights(property, "_small", "_medium", "_large") + EditorGUIUtility.singleLineHeight;
        }
    }
}
