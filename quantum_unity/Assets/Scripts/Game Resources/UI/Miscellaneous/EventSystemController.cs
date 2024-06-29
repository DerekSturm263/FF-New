using Extensions.Components.Miscellaneous;
using Extensions.Components.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class EventSystemController : Controller<EventSystemController>
{
    public class SavableData
    {
        private Dictionary<Selectable, Navigation> _selectables = new();
        private Dictionary<InputEvent, bool> _inputEvents = new();
        private EventSystem _oldEventSystem;

        public void Enable()
        {
            DisableSelectables();
            DisableInputEvents();
            DisableOldEventSystem();
        }

        public void Disable()
        {
            EnableOldEventSystem();
            EnableInputEvents();
            EnableSelectables();
        }

        private void DisableOldEventSystem()
        {
            if (EventSystem.current)
            {
                _oldEventSystem = EventSystem.current;
                _oldEventSystem.enabled = false;
            }
        }

        private void EnableOldEventSystem()
        {
            if (_oldEventSystem)
            {
                _oldEventSystem.enabled = true;
                EventSystem.current = _oldEventSystem;
            }
        }

        private void DisableSelectables()
        {
            _selectables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToDictionary(item => item, item => item.navigation);

            foreach (Selectable selectable in _selectables.Keys)
            {
                selectable.navigation = new() { mode = Navigation.Mode.None };
            }
        }

        private void EnableSelectables()
        {
            foreach (var kvp in _selectables)
            {
                kvp.Key.navigation = kvp.Value;
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

        private void EnableInputEvents()
        {
            foreach (var kvp in _inputEvents)
            {
                kvp.Key.enabled = kvp.Value;
            }
        }
    }

    [NonSerialized] private readonly Stack<SavableData> _data = new();

    public void Enable()
    {
        SavableData data = new();
        data.Enable();

        _data.Push(data);
    }

    public void Disable()
    {
        _data.Pop().Disable();
    }
}
