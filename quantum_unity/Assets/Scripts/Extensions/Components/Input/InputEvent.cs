using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Extensions.Components.Input
{
    public class InputEvent : MonoBehaviour
    {
        [SerializeField] protected InputButton _button;
        public InputButton Button => _button;
        public void SetButton(InputButton button) => _button = button;

        private InputAction _action;

        [SerializeField] private bool _readContinuous;
        [SerializeField] private bool _processWhenEmpty = true;

        [SerializeField] private GameObject _parent;
        [SerializeField] private bool _requiresFocus;

        [SerializeField] protected UnityEvent _onAction;
        public UnityEvent OnAction => _onAction;

        [SerializeField] protected UnityEvent<float> _onAxisAction;
        public UnityEvent<float> OnAxisAction => _onAxisAction;

        [SerializeField] protected UnityEvent<Vector2> _onVector2Action;
        public UnityEvent<Vector2> OnVector2Action => _onVector2Action;

        [SerializeField] protected UnityEvent<InputAction.CallbackContext> _onCtxAction;
        public UnityEvent<InputAction.CallbackContext> OnCtxAction => _onCtxAction;

        protected bool _isReady;

        private void Awake()
        {
            _action = _button.Action.Clone();

            if (_readContinuous)
                return;

            _action.performed += Type switch
            {
                "Axis" => ctx => { if (HasFocus() && !IsInputting()) Invoke(ctx.ReadValue<float>()); },
                "Vector2" => ctx => { if (HasFocus() && !IsInputting()) Invoke(ctx.ReadValue<Vector2>()); },
                _ => ctx => { if (HasFocus() && !IsInputting()) Invoke(); }
            };

            _action.performed += _onCtxAction.Invoke;
        }

        private void Update()
        {
            if (!_readContinuous)
                return;

            switch (Type)
            {
                case "Axis":
                    float value = _action.ReadValue<float>();
                    if ((_processWhenEmpty || value != 0) && HasFocus() && !IsInputting())
                        Invoke(value);

                    break;

                case "Vector2":
                    Vector2 value2 = _action.ReadValue<Vector2>();
                    if ((_processWhenEmpty || value2 != Vector2.zero) && HasFocus() && !IsInputting())
                        Invoke(value2);

                    break;

                default:
                    if (_action.IsPressed() && HasFocus() && !IsInputting())
                        Invoke();
                    
                    break;
            }
        }

        private void OnEnable()
        {
            _action.Enable();
            Invoke(nameof(SetReady), 0.05f);
        }

        private void OnDisable()
        {
            _isReady = false;
            _action.Disable();
        }

        private void SetReady()
        {
            _isReady = true;
        }

        public bool HasFocus()
        {
            return !_requiresFocus || EventSystem.current.currentSelectedGameObject == _parent;
        }

        public static bool IsInputting()
        {
            bool isUsingKeyboard = InputMapperController.Instance.CurrentDevice.displayName.Equals("Keyboard");
            if (!isUsingKeyboard)
                return false;

            GameObject selected = EventSystem.current?.currentSelectedGameObject;
            if (!selected)
                return false;

            if (!EventSystem.current.currentSelectedGameObject.TryGetComponent(out TMPro.TMP_InputField inputField))
                return false;

            return inputField.isFocused;
        }

        public void Invoke() => _onAction.Invoke();
        public void Invoke(float arg0) => _onAxisAction.Invoke(arg0);
        public void Invoke(Vector2 arg0) => _onVector2Action.Invoke(arg0);

        public string Type => _button && _button.Action is not null ? _button.Action.expectedControlType : string.Empty;
    }
}
