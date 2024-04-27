using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Color Picker", 13)]
    public class ColorPicker : Selectable
    {
        [SerializeField] private ColorPickerEvent _onValueChanged;
        public ColorPickerEvent OnValueChanged => _onValueChanged;

        [System.Serializable]
        public class ColorPickerEvent : UnityEvent<Color> { }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Color Picker", priority = 30)]
        private static void Create_Internal(UnityEditor.MenuCommand cmd)
        {
            GameObject obj = new("Color Picker");

            UnityEditor.GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            UnityEditor.Selection.activeGameObject = obj;

            obj.AddComponent<ColorPicker>();
        }

#endif
    }
}