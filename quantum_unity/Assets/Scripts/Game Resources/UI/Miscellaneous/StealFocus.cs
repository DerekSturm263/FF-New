using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class StealFocus : MonoBehaviour
    {
        private IEnumerable<Extensions.Components.Input.InputEvent> _inputEvents;
        private Dictionary<Extensions.Components.Input.InputEvent, bool> _wasEnabled;

        private GameObject _oldSelected;
        private List<(Selectable, bool)> _allSelectables;

        private void OnEnable()
        {
            RememberOldSelected();
            _allSelectables = FindObjectsByType<Selectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select<Selectable, (Selectable, bool)>(item => new(item, item.interactable)).ToList();
            foreach (var selectable in _allSelectables)
            {
                selectable.Item1.interactable = false;
            }

            _inputEvents = FindObjectsByType<Extensions.Components.Input.InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(item => item.gameObject != gameObject);
            _wasEnabled = new();

            foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
            {
                _wasEnabled.Add(inputEvent, inputEvent.enabled);
                inputEvent.enabled = false;
            }
        }

        private void OnDisable()
        {
            foreach (Extensions.Components.Input.InputEvent inputEvent in _inputEvents)
            {
                inputEvent.enabled = _wasEnabled[inputEvent];
            }

            foreach (var selectable in _allSelectables)
            {
                selectable.Item1.interactable = selectable.Item2;
            }

            SetOldSelected();
        }

        public void RememberOldSelected()
        {
            if (EventSystem.current)
                _oldSelected = EventSystem.current.currentSelectedGameObject;
        }

        public void SetOldSelected()
        {
            if (EventSystem.current && _oldSelected)
                EventSystem.current.SetSelectedGameObject(_oldSelected);
        }
    }
}
