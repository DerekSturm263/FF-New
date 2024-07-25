using UnityEditor;
using UnityEngine;
using Extensions.Types;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Nullable<>))]
    internal class NullableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty prop = property.FindPropertyRelative("_hasValue");
            prop.boolValue = EditorGUI.Toggle(new Rect(position.x, position.y, (EditorGUIUtility.singleLineHeight), position.height), prop.boolValue);

            EditorGUI.BeginDisabledGroup(!prop.boolValue);
            EditorGUI.PropertyField(new Rect(position.x + (EditorGUIUtility.singleLineHeight + 4), position.y, position.width - (EditorGUIUtility.singleLineHeight + 4), position.height), property.FindPropertyRelative("_nonNullValue"), label);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_nonNullValue"));
        }
    }
}
