using UnityEditor;
using UnityEngine;
using Extensions.Types;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Tuple<,>))]
    internal class TupleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, (position.width / 2) - 5, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("_item1"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + (position.width / 2) + 5, position.y + EditorGUIUtility.singleLineHeight, (position.width / 2) - 5, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("_item2"), GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Mathf.Max(EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_item1")), EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_item2"))) + EditorGUIUtility.singleLineHeight;
        }
    }
}
