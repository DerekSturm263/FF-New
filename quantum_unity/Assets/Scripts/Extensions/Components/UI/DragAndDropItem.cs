using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Drag and Drop Item", 13)]
    public class DragAndDropItem : Selectable, IPointerUpHandler, IPointerDownHandler, ISubmitHandler
    {
        private static DragAndDropSlot _currentSelectedSlot;
        public static void SetCurrentSelectedSlot(DragAndDropSlot slot) => _currentSelectedSlot = slot;

        [SerializeField] private bool _createCopyOnDrag;
        public bool CreateCopyOnDrag => _createCopyOnDrag;

        [SerializeField] private bool _returnToOriginalPositionUponFailure;
        public bool ReturnToOriginalPositionUponFailure => _returnToOriginalPositionUponFailure;

        [SerializeField] private UnityEvent<DragAndDropSlot> _onRelease;
        public UnityEvent<DragAndDropSlot> OnRelease => _onRelease;
        public void InvokeOnRelease<T>(DragAndDropSlot slot) where T : class => _onRelease.Invoke(slot);

        private RectTransform _rect;
        private RectTransform _parentRect;
        private InputSystemUIInputModule _inputModule;

        private DragAndDropItem _original;
        private bool _isCopy;
        private bool _isDragging;
        private Vector2 _originalPos;
        private Vector2 _dragPos;
        private bool _isGamepadDragging;
        private Canvas _parentCanvas;

        private object _value;
        public object Value => _value;
        public void SetValue(object value) => _value = value;

        protected override void Awake()
        {
            base.Awake();

            _rect = GetComponent<RectTransform>();
            _parentRect = transform.parent.GetComponent<RectTransform>();
            _inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            if (_isGamepadDragging)
                _dragPos = _parentCanvas.transform.InverseTransformPoint(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).position);
            else
                _dragPos = _inputModule.point.action.ReadValue<Vector2>();

            if (_isDragging)
            {
                if (_isGamepadDragging)
                    DragGamepad(_dragPos);
                else
                    DragMouse(_dragPos);

                if (_inputModule.leftClick.action.WasReleasedThisFrame() || _inputModule.submit.action.WasReleasedThisFrame())
                    Drop();
            }
        }

        private void DragMouse(Vector2 position)
        {
            _rect.localPosition = ScreenToCanvasPoint(position) - _rect.sizeDelta / 2;
        }

        private void DragGamepad(Vector2 position)
        {
            _rect.localPosition = position - new Vector2(_rect.sizeDelta.x, 0);
        }

        private Vector2 ScreenToCanvasPoint(Vector2 originalPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, originalPosition, UnityEngine.Camera.main, out Vector2 position))
                return position;

            return _originalPos;
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (!interactable || _isCopy)
                return;

            StartDrag(true);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable || _isCopy || eventData.button != PointerEventData.InputButton.Left)
                return;

            StartDrag(false);
        }

        private void StartDrag(bool isGamepadDragging)
        {
            if (_createCopyOnDrag)
            {
                DragAndDropItem copy = Instantiate(this, _parentCanvas.transform);

                copy._originalPos = _rect.position;
                copy._isDragging = true;
                copy._rect.sizeDelta = _rect.sizeDelta;
                copy._isCopy = true;
                copy._original = this;
                copy.image.raycastTarget = false;
                copy._onRelease = _onRelease;
                copy._isGamepadDragging = isGamepadDragging;
                copy.navigation = new Navigation() { mode = Navigation.Mode.None };
                copy._value = _value;

                if (isGamepadDragging)
                    copy.DragGamepad(_dragPos);
                else
                    copy.DragMouse(_dragPos);

                if (isGamepadDragging)
                    EventSystem.current.SetSelectedGameObject(GameObject.FindGameObjectWithTag("Container").transform.GetChild(0).gameObject);
            }
            else
            {
                _originalPos = _rect.position;
                _isDragging = true;
                _isGamepadDragging = isGamepadDragging;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!_isDragging || eventData.button != PointerEventData.InputButton.Left)
                return;

            Drop();
        }

        private void Drop()
        {
            _isDragging = false;

            if (!_currentSelectedSlot)
            {
                if (_isCopy)
                {
                    SetEnabled(true);
                    Destroy(gameObject);
                }
                else if (_returnToOriginalPositionUponFailure)
                {
                    _rect.position = _originalPos;
                }

                return;
            }

            _currentSelectedSlot.SetValue(_value);
            _onRelease.Invoke(_currentSelectedSlot);
            _currentSelectedSlot.InvokeOnSlotEnter();

            transform.SetParent(_currentSelectedSlot.transform);
            transform.localPosition = Vector2.zero;

            _currentSelectedSlot = null;
            
            if (_isCopy)
            {
                Destroy(gameObject);
            }
        }

        public void SetEnabled(bool isEnabled)
        {
            if (_isCopy)
                _original.interactable = isEnabled;
            else
                interactable = isEnabled;
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Drag and Drop Item", priority = 30)]
        private static void Create_Internal(UnityEditor.MenuCommand cmd)
        {
            GameObject obj = new("Drag and Drop Item");

            UnityEditor.GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            UnityEditor.Selection.activeGameObject = obj;

            obj.AddComponent<DragAndDropItem>();
        }

#endif
    }
}
