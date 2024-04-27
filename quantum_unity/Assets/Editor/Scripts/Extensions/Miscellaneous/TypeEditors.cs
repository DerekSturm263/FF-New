using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.Extensions.Miscellaneous
{
    public static class TypeEditors
    {
        private static readonly Dictionary<System.Type, System.Func<Rect, SerializedProperty, object>> _typeDictionary = new()
        {
            [typeof(AnimationCurve)] = (Rect rect, SerializedProperty prop) => EditorGUI.CurveField(rect, "", (AnimationCurve)prop.boxedValue),
            [typeof(bool)] = (Rect rect, SerializedProperty prop) => EditorGUI.Toggle(rect, " ", (bool)prop.boxedValue),
            [typeof(Color)] = (Rect rect, SerializedProperty prop) => EditorGUI.ColorField(rect, " ", (Color)prop.boxedValue),
            [typeof(double)] = (Rect rect, SerializedProperty prop) => EditorGUI.DoubleField(rect, " ", (double)prop.boxedValue),
            [typeof(int)] = (Rect rect, SerializedProperty prop) => EditorGUI.IntField(rect, " ", (int)prop.boxedValue),
            [typeof(long)] = (Rect rect, SerializedProperty prop) => EditorGUI.LongField(rect, " ", (long)prop.boxedValue),
            [typeof(Gradient)] = (Rect rect, SerializedProperty prop) => EditorGUI.GradientField(rect, " ", (Gradient)prop.boxedValue),
            [typeof(float)] = (Rect rect, SerializedProperty prop) => EditorGUI.FloatField(rect, " ", (float)prop.boxedValue),
            [typeof(string)] = (Rect rect, SerializedProperty prop) => EditorGUI.TextField(rect, " ", (string)prop.boxedValue),
            [typeof(Vector2)] = (Rect rect, SerializedProperty prop) => EditorGUI.Vector2Field(rect, " ", (Vector2)prop.boxedValue),
            [typeof(Vector2Int)] = (Rect rect, SerializedProperty prop) => EditorGUI.Vector2IntField(rect, " ", (Vector2Int)prop.boxedValue),
            [typeof(Vector3)] = (Rect rect, SerializedProperty prop) => EditorGUI.Vector3Field(rect, " ", (Vector3)prop.boxedValue),
            [typeof(Vector3Int)] = (Rect rect, SerializedProperty prop) => EditorGUI.Vector3IntField(rect, " ", (Vector3Int)prop.boxedValue),
            [typeof(Vector4)] = (Rect rect, SerializedProperty prop) => EditorGUI.Vector4Field(rect, " ", (Vector4)prop.boxedValue)
        };

        public static Dictionary<System.Type, System.Func<Rect, SerializedProperty, object>> TypeDictionary => _typeDictionary;
    }
}
