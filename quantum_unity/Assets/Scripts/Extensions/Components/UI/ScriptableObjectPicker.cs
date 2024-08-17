using GameResources.UI.Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/ScriptableObject Picker", 13)]
    public abstract class ScriptableObjectPicker<T, TScriptableObjectPopulator> : Selectable, ISubmitHandler, IPointerClickHandler where TScriptableObjectPopulator : Populate<T>
    {
        [SerializeField] private string _label;
        [SerializeField] private TScriptableObjectPopulator _populator;
        [SerializeField] private GameObject _popupTemplate;

        [SerializeField] private ScriptableObjectPickerEvent _onValueHovered;
        public ScriptableObjectPickerEvent OnValueHovered => _onValueHovered;

        [SerializeField] private ScriptableObjectPickerEvent _onValueChanged;
        public ScriptableObjectPickerEvent OnValueChanged => _onValueChanged;

        [System.Serializable]
        public class ScriptableObjectPickerEvent : UnityEvent<T> { }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            ComponentPopupController<TScriptableObjectPopulator, T>.Spawn(_label, _popupTemplate, item =>
            {
                item.SubscribeOnButtonHover(value => _onValueHovered.Invoke(value));
                item.SubscribeOnButtonClick(value => _onValueChanged.Invoke(value));
            }, _populator);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ComponentPopupController<TScriptableObjectPopulator, T>.Spawn(_label, _popupTemplate, item =>
            {
                item.SubscribeOnButtonHover(value => _onValueHovered.Invoke(value));
                item.SubscribeOnButtonClick(value => _onValueChanged.Invoke(value));
            }, _populator);
        }
    }
}
