using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    public class UIController : Miscellaneous.Controller<UIController>
    {
        [SerializeField] private Material _glow;

        private readonly Dictionary<GameObject, GameObject> _selectedElement = new();
        private PopulateBase[] _populaters;

        public override void Initialize()
        {
            base.Initialize();

            PopulateAllAwakePopulaters();
        }

        public override void Shutdown()
        {
            RemoveAllAwakePopulaters();

            base.Shutdown();
        }

        public void DeselectAllElements()
        {
            if (EventSystem.current)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void StoreSelectedElement(GameObject key)
        {
            if (EventSystem.current)
            {
                if (!_selectedElement.ContainsKey(key))
                    _selectedElement.Add(key, EventSystem.current.currentSelectedGameObject);
                else
                    _selectedElement[key] = EventSystem.current.currentSelectedGameObject;
            }
        }
        public void RestoreSelectedElement(GameObject key)
        {
            if (EventSystem.current && _selectedElement.ContainsKey(key))
            {
                EventSystem.current.SetSelectedGameObject(_selectedElement[key]);
            }
        }

        public void ButtonGlow(Button button)
        {
            button.image.color = Color.white;
            button.image.material = _glow;
        }

        public void ButtonUnglow(Button button)
        {
            button.image.color = new(1, 1, 1, 0.25f);
            button.image.material = null;
        }

        private void PopulateAllAwakePopulaters()
        {
            _populaters = FindObjectsByType<PopulateBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (PopulateBase populater in _populaters)
            {
                if (populater.LoadingType == UI.PopulateBase.LoadStage.Awake && !populater.IsInitialized)
                    populater.LoadAllItems();
            }
        }

        private void RemoveAllAwakePopulaters()
        {
            foreach (PopulateBase populater in _populaters)
            {
                populater.RemoveFromList();
            }
        }

        public void SetAnimBoolOn(Animator anim)
        {
            anim.SetBool("On", true);
        }

        public void SetAnimBoolOff(Animator anim)
        {
            anim.SetBool("On", false);
        }
    }
}
