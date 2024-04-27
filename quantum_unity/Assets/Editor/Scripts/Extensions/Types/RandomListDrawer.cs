using UnityEditor;
using UnityEngine;
using Extensions.Types;
using static Editor.Extensions.Miscellaneous.EditorHelper;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(RandomList<>))]
    internal class RandomListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            DrawProperty(ref position, property, "_list", false, label.text);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CombinePropertyHeights(property, "_list");
        }
    }
}
