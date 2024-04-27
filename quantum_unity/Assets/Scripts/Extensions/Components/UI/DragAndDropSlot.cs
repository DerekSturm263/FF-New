using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Drag and Drop Slot", 14)]
    [ExecuteAlways]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class DragAndDropSlot : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        [SerializeField] private UnityEvent<object> _onSlotEnter;
        public void InvokeOnSlotEnter() => _onSlotEnter?.Invoke(_value);

        [SerializeField] private UnityEvent<object> _onSlotRemove;
        public void InvokeOnSlotRemove() => _onSlotRemove?.Invoke(_value);

        protected Image _itemIcon;
        protected TMPro.TMP_Text _itemName;

        protected object _value;
        public object Value => _value;
        public void SetValue(object value) => _value = value;

        protected override void Awake()
        {
            _itemIcon = GetComponentInChildren<Image>();
            _itemName = GetComponentInChildren<TMPro.TMP_Text>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void SwapSlotValues(DragAndDropSlot otherSlot)
        {
            InvokeOnSlotRemove();
            otherSlot.InvokeOnSlotRemove();

            Extensions.Miscellaneous.Helper.Swap(ref _value, ref otherSlot._value);

            otherSlot.InvokeOnSlotEnter();
            InvokeOnSlotEnter();
        }

        public void OnSelect(BaseEventData eventData) => DragAndDropItem.SetCurrentSelectedSlot(this);
        public void OnDeselect(BaseEventData eventData) => DragAndDropItem.SetCurrentSelectedSlot(null);

        public void OnPointerEnter(PointerEventData eventData) => DragAndDropItem.SetCurrentSelectedSlot(this);
        public void OnPointerExit(PointerEventData eventData) => DragAndDropItem.SetCurrentSelectedSlot(null);

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Drag and Drop Slot", priority = 30)]
        private static void Create_Internal(UnityEditor.MenuCommand cmd)
        {
            GameObject obj = new("Drag and Drop Slot");

            UnityEditor.GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            UnityEditor.Selection.activeGameObject = obj;

            obj.AddComponent<DragAndDropSlot>();
        }

#endif
    }
}
