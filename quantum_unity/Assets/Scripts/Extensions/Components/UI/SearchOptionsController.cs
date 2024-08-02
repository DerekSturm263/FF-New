using Extensions.Components.UI;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class SearchOptionsController : SpawnableController<PopulateBase>
    {
        [SerializeField] private GameObject _dropdownTemplate;
        [SerializeField] private GameObject _buttonTemplate;

        private Action _filterAction, _groupAction, _sortAction;

        public void Spawn()
        {
            Spawn(default);
        }

        protected override void SetUp(PopulateBase populateBase)
        {
            var layoutGroups = _templateInstance.GetComponentsInChildren<LayoutGroup>();

            GameObject filter = Instantiate(_dropdownTemplate, layoutGroups[0].transform);

            TMPro.TMP_Text text = filter.GetComponentInChildren<TMPro.TMP_Text>();
            text.SetText("Filter Mode");

            TMPro.TMP_Dropdown filterDropdown = filter.GetComponentInChildren<TMPro.TMP_Dropdown>();

            filterDropdown.ClearOptions();
            filterDropdown.AddOptions(populateBase.AllFilterModeNames);
            filterDropdown.SetValueWithoutNotify(populateBase.GetCurrentFilterModeIndex());
            filterDropdown.onValueChanged.AddListener(value => _filterAction = () => populateBase.SetFilterModeAtIndex(value));

            GameObject group = Instantiate(_dropdownTemplate, layoutGroups[0].transform);

            TMPro.TMP_Text text2 = group.GetComponentInChildren<TMPro.TMP_Text>();
            text2.SetText("Group Mode");

            TMPro.TMP_Dropdown groupDropdown = group.GetComponentInChildren<TMPro.TMP_Dropdown>();

            groupDropdown.ClearOptions();
            groupDropdown.AddOptions(populateBase.AllGroupModeNames);
            groupDropdown.SetValueWithoutNotify(populateBase.GetCurrentGroupModeIndex());
            groupDropdown.onValueChanged.AddListener(value => _groupAction = () => populateBase.SetGroupModeAtIndex(value));

            GameObject sort = Instantiate(_dropdownTemplate, layoutGroups[0].transform);

            TMPro.TMP_Text text3 = sort.GetComponentInChildren<TMPro.TMP_Text>();
            text3.SetText("Sort Mode");

            TMPro.TMP_Dropdown sortDropdown = sort.GetComponentInChildren<TMPro.TMP_Dropdown>();

            sortDropdown.ClearOptions();
            sortDropdown.AddOptions(populateBase.AllSortModeNames);
            sortDropdown.SetValueWithoutNotify(populateBase.GetCurrentSortModeIndex());
            sortDropdown.onValueChanged.AddListener(value => _sortAction = () => populateBase.SetSortModeAtIndex(value));

            GameObject button1 = Instantiate(_buttonTemplate, layoutGroups[1].transform);

            TMPro.TMP_Text text4 = button1.GetComponentInChildren<TMPro.TMP_Text>();
            text4.SetText("Apply");

            Button apply = button1.GetComponent<Button>();
            apply.onClick.AddListener(() => { ApplySettings(); Despawn(); });

            GameObject button2 = Instantiate(_buttonTemplate, layoutGroups[1].transform);

            TMPro.TMP_Text text5 = button2.GetComponentInChildren<TMPro.TMP_Text>();
            text5.SetText("Cancel");

            Button cancel = button2.GetComponent<Button>();
            cancel.onClick.AddListener(Despawn);
        }

        public void ApplySettings()
        {
            _filterAction?.Invoke();
            _groupAction?.Invoke();
            _sortAction?.Invoke();
        }
    }
}
