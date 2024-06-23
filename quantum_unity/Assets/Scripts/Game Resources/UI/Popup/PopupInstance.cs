using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class PopupInstance : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onAnimationEndStart;
        public void InvokeOnAnimationEndStart() => _onAnimationEndStart.Invoke();

        private IEnumerable<Extensions.Components.Input.InputEvent> _inputEvents;
        private Dictionary<Extensions.Components.Input.InputEvent, bool> _wasEnabled;

        private Button[] _popupButtons;

        private void Start()
        {
            _popupButtons = GetComponentsInChildren<Button>();
        }

        private void OnEnable()
        {
            _inputEvents = FindObjectsByType<Extensions.Components.Input.InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(item => item.gameObject != gameObject);
            _wasEnabled = new();

            foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
            {
                _wasEnabled.Add(inputEvent, inputEvent.enabled);
                inputEvent.enabled = false;
            }
        }

        public void ReenableInputEvents()
        {
            foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
            {
                inputEvent.enabled = _wasEnabled[inputEvent];
            }
        }

        public void DisableButtons()
        {
            foreach (Button button in _popupButtons)
            {
                button.interactable = false;
            }
        }
    }
}
