using UnityEditor;
using UnityEngine;
using Extensions.Types;
using static Editor.Extensions.Miscellaneous.EditorHelper;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Range<>))]
    internal class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

            Rect indentedRect = new(position.position + new Vector2(10, EditorGUIUtility.singleLineHeight), position.size + new Vector2(-10, 0));

            DrawProperty(ref indentedRect, property, "_min");
            DrawProperty(ref indentedRect, property, "_max");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CombinePropertyHeights(property, "_min", "_max") + EditorGUIUtility.singleLineHeight;
        }
    }
}
