using Extensions.Components.UI;
using Extensions.Miscellaneous;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopulateAsset<T> : Populate<T, int> where T : InfoAssetAsset
{
    [SerializeField] private bool _showCheckmarks;

    private GameObject _oldSelected;

    protected abstract string FilePath();

    protected override Sprite Icon(T item) => item.Icon;

    protected override IEnumerable<T> LoadAll() => Resources.LoadAll<T>(FilePath());

    protected override string Name(T item) => item.name;

    protected override Func<T, int> Sort() => (item) => item.SortingID;

    protected abstract bool HasEquipped(T item);

    protected override void Decorate(GameObject buttonObj, T item)
    {
        base.Decorate(buttonObj, item);

        if (!_showCheckmarks)
            return;

        bool isEquipped = HasEquipped(item);
        buttonObj.FindChildWithTag("Checkmark", true)?.SetActive(isEquipped);

        if (isEquipped)
        {
            _oldSelected = buttonObj;
        }
    }

    protected override void SetEvents(GameObject buttonObj, T item)
    {
        base.SetEvents(buttonObj, item);

        if (!_showCheckmarks)
            return;

        Button button = buttonObj.GetComponentInChildren<Button>();
        if (button && GiveEvents(item))
        {
            button.onClick.AddListener(() =>
            {
                if (_oldSelected == buttonObj)
                    return;

                buttonObj.FindChildWithTag("Checkmark", true)?.SetActive(true);

                if (_oldSelected)
                    _oldSelected.FindChildWithTag("Checkmark", true)?.SetActive(false);

                _oldSelected = buttonObj;
            });
        }
    }
}
