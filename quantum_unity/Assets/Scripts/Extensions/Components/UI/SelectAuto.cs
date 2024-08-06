using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class SelectAuto : UIBehaviour
{
    private static int _selectedIndex = -1;

    [System.Flags]
    public enum SelectType
    {
        Index = 1 << 0,
        Old = 1 << 1,
        First = 1 << 2,
        Last = 1 << 3
    }

    [SerializeField] private SelectType _selectMethod;
    [SerializeField] private List<Selectable> _dontSelect;
    [SerializeField] private bool _resetIndexOnStart = true;
    [SerializeField] private bool _executeOnSelect = true;
    [SerializeField] private float _delay;

    private Selectable _oldSelected;

    protected override void Start()
    {
        if (_resetIndexOnStart)
            _selectedIndex = -1;
    }

    protected override void OnEnable()
    {
        Invoke(nameof(SetSelectedItem), _delay);
    }

    protected override void OnDisable()
    {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject)
            _oldSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

        if (_oldSelected)
            _selectedIndex = _oldSelected.transform.GetSiblingIndex();
        else
            _selectedIndex = -1;
    }

    public void SetSelectedItem() => SetSelectedItem(_selectMethod);

    public void SetSelectedItem(SelectType selectMethod)
    {
        if (transform.childCount == 0)
            return;

        Selectable selected = null;

        if (selectMethod.HasFlag(SelectType.Index) && _selectedIndex != -1)
        {
            if (_selectedIndex >= transform.childCount)
                selected = transform.GetChild(transform.childCount - 1).GetComponentInChildren<Selectable>();
            else
                selected = transform.GetChild(_selectedIndex).GetComponentInChildren<Selectable>();
        }
        else if (selectMethod.HasFlag(SelectType.Old) && _oldSelected && _oldSelected.interactable && !_dontSelect.Contains(_oldSelected))
        {
            selected = _oldSelected;
        }

        if (!selected || !selected.GetComponentInChildren<Selectable>().interactable)
        {
            if (selectMethod.HasFlag(SelectType.First))
            {
                foreach (Selectable selectable in transform.GetComponentsInChildren<Selectable>())
                {
                    if (selectable.interactable && !_dontSelect.Contains(selectable))
                    {
                        selected = selectable;
                        break;
                    }
                }
            }
            else if (selectMethod.HasFlag(SelectType.Last))
            {
                foreach (Selectable selectable in transform.GetComponentsInChildren<Selectable>().Reverse())
                {
                    if (selectable.interactable && !_dontSelect.Contains(selectable))
                    {
                        selected = selectable;
                        break;
                    }
                }
            }
        }

        if (selected)
        {
            MultiplayerEventSystem eventSystem = GetComponentInParent<MultiplayerEventSystem>();
            if (eventSystem)
            {
                eventSystem.SetSelectedGameObject(selected.gameObject);

                Selector selector = GetComponentInParent<CustomMultiplayerUIInputSystemModule>().Selector;
                if (selector)
                    selector.ChildToSelected(selected.gameObject);
            }
            else
                EventSystem.current.SetSelectedGameObject(selected.gameObject);

            if (_executeOnSelect && selected.TryGetComponent(out EventTrigger eventTrigger))
                eventTrigger.OnSelect(new(EventSystem.current));
        }
    }
}
