using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Color Picker", 13)]
    public class ColorPicker : Selectable, ISubmitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject _colorPickerPopup;
        [SerializeField] private Image[] _colorPickerColorPreviews;

        [SerializeField] private ColorPickerEvent _onValueChanged;
        public ColorPickerEvent OnValueChanged => _onValueChanged;

        [System.Serializable]
        public class ColorPickerEvent : UnityEvent<Color> { }

        private float _hue;
        private float _saturation;
        private float _value;
        private Color _color;

        void ISubmitHandler.OnSubmit(UnityEngine.EventSystems.BaseEventData eventData)
        {
            SpawnColorPicker();
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            SpawnColorPicker();
        }

        public void SpawnColorPicker()
        {
            _colorPickerPopup.SetActive(true);
        }

        public void DespawnColorPicker()
        {
            _colorPickerPopup.SetActive(false);
        }

        public void ConfirmColor()
        {
            _colorPickerColorPreviews[1].color = _color;
            _onValueChanged.Invoke(_color);
        }

        public void SetHue(float value)
        {
            _hue = value;
            _color = Color.HSVToRGB(_hue, _saturation, _value);

            _colorPickerColorPreviews[0].color = _color;
        }

        public void SetSaturation(float value)
        {
            _saturation = value;
            _color = Color.HSVToRGB(_hue, _saturation, _value);

            _colorPickerColorPreviews[0].color = _color;
        }

        public void SetValue(float value)
        {
            _value = value;
            _color = Color.HSVToRGB(_hue, _saturation, _value);

            _colorPickerColorPreviews[0].color = _color;
        }

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