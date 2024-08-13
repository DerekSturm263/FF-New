using Extensions.Components.Miscellaneous;
using Extensions.Components.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventSystemController : Controller<EventSystemController>
{
    [SerializeField] private UnityEvent _onPopupOpen;
    public void InvokeOnPopupOpen() => _onPopupOpen?.Invoke();

    [SerializeField] private UnityEvent _onPopupClose;
    public void InvokeOnPopupClose() => _onPopupClose?.Invoke();

    [System.Serializable]
    public struct SavableData
    {
        private Dictionary<Selectable, (bool, Navigation)> _selectables;
        private Dictionary<InputEvent, bool> _inputEvents;
        private EventSystem _oldEventSystem;
        private GameObject _oldSelected;

        public SavableData(int _)
        {
            _selectables = new();
            _inputEvents = new();
            _oldEventSystem = null;
            _oldSelected = null;
        }

        public void Enable()
        {
            DisableSelectables();
            DisableInputEvents();
            DisableOldEventSystem();
        }

        public readonly void Disable()
        {
            EnableOldEventSystem();
            EnableInputEvents();
            EnableSelectables();
        }

        private unsafe void DisableOldEventSystem()
        {
            if (EventSystem.current)
            {
                _oldEventSystem = EventSystem.current;

                _oldSelected = _oldEventSystem.currentSelectedGameObject;
                _oldEventSystem.gameObject.SetActive(false);
            }
        }

        private readonly unsafe void EnableOldEventSystem()
        {
            if (_oldEventSystem)
            {
                _oldEventSystem.gameObject.SetActive(true);
                EventSystem.current = _oldEventSystem;

                if (_oldSelected)
                    EventSystem.current.SetSelectedGameObject(_oldSelected);
            }
        }

        private void DisableSelectables()
        {
            _selectables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToDictionary(item => item, item => (item.interactable, item.navigation));

            foreach (Selectable selectable in _selectables.Keys)
            {
                selectable.navigation = new() { mode = Navigation.Mode.None };
            }
        }

        private readonly void EnableSelectables()
        {
            foreach (var kvp in _selectables)
            {
                if (kvp.Key)
                    kvp.Key.navigation = kvp.Value.Item2;
            }
        }

        private void DisableInputEvents()
        {
            _inputEvents = FindObjectsByType<InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToDictionary(item => item, item => item.enabled);

            foreach (InputEvent inputEvent in _inputEvents.Keys)
            {
                inputEvent.enabled = false;
            }
        }

        private readonly void EnableInputEvents()
        {
            foreach (var kvp in _inputEvents)
            {
                if (kvp.Key)
                    kvp.Key.enabled = kvp.Value;
            }
        }
    }

    private List<SavableData> _data;

    public override void Initialize()
    {
        base.Initialize();

        _data = new();
    }

    public void Enable()
    {
        SavableData data = new(0);
        data.Enable();

        if (_data.Count == 0)
            InvokeOnPopupOpen();

        _data.Add(data);

        Debug.Log($"Enabled! Old: {_data.Count - 1}, New: {_data.Count}");
    }

    public void Disable()
    {
        _data[^1].Disable();
        _data.RemoveAt(_data.Count - 1);

        if (_data.Count == 0)
            InvokeOnPopupClose();

        Debug.Log($"Disabled! Old: {_data.Count + 1}, New: {_data.Count}");
    }

    public void PushEmpty()
    {
        _data.Add(new());
    }

    public void PopEmpty()
    {
        _data.RemoveAt(_data.Count - 1);
    }
}
