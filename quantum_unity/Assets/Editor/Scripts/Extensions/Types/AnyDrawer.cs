using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Extensions.Types;

namespace Editor.Extensions.Types
{
    [CustomPropertyDrawer(typeof(Any))]
    internal class AnyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

            SerializedProperty typeName = property.FindPropertyRelative("_typeName");
            if (string.IsNullOrEmpty(typeName.stringValue))
                typeName.stringValue = typeof(int).AssemblyQualifiedName;

            System.Type type = System.Type.GetType(typeName.stringValue);

            List<System.Type> typeList = Miscellaneous.TypeEditors.TypeDictionary.Select(item => item.Key).ToList();
            int typeIndex = typeList.IndexOf(type);

            Rect typePosition = new(position.x + 200, position.y, position.width - 200, EditorGUIUtility.singleLineHeight);
            int selectedType = EditorGUI.Popup(typePosition, "", typeIndex, typeList.Select(item => item.Name).ToArray());

            if (selectedType != typeIndex)
            {
                typeName.stringValue = typeList[selectedType].AssemblyQualifiedName;
                type = System.Type.GetType(typeName.stringValue);
            }

            Rect valuePosition = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PrefixLabel(valuePosition, new("Value"));

            if (Miscellaneous.TypeEditors.TypeDictionary.TryGetValue(type, out System.Func<Rect, SerializedProperty, object> action))
            {
                SerializedProperty propertyType = property.FindPropertyRelative("_type");
                SerializedProperty value;

                if ((Any.PropertyType)propertyType.enumValueIndex == Any.PropertyType.Object)
                {
                    value = property.FindPropertyRelative("_objValue");

                    try     { value.boxedValue = action(valuePosition, value);    }
                    catch   { value.boxedValue = Any.GetDefault(type);            }
                }
                else
                {
                    value = property.FindPropertyRelative("_unityObjValue");

                    try     { action(valuePosition, value);                                }
                    catch   { value.objectReferenceValue = Any.GetDefault(type) as Object; }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property) + 8;
        }
    }
}
