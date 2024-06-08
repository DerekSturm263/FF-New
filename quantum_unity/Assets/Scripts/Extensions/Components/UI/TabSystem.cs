using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/TextMeshPro - Tab System", 12)]
    [DisallowMultipleComponent]
    public class TabSystem : UIBehaviour
    {
        private static readonly Dictionary<string, int> _allIndices = new();

        [SerializeField] private Button[] _tabButtons;
        [SerializeField] private Transform _tabLists;
        [SerializeField] private RectTransform _selectionMarker;
        [SerializeField] private float _selectionMarkerSpeed = 20;

        [SerializeField] private bool _rememberLastTab;
        [SerializeField] private bool _cycle = true;
        [SerializeField] private string _dictionaryIndex;
        [SerializeField] private int _defaultTab;

        [SerializeField] private UnityEvent<Button> _onTabSelection;
        [SerializeField] private UnityEvent<Button> _onTabDeselection;

        [Header("Move Tab Settings")]
        [SerializeField] private float[] _positions;

        private int _currentTabIndex;
        private readonly Dictionary<Button, (GameObject gameObject, int index)> _tabs = new();

        protected override void Awake()
        {
            _tabs.Clear();

            if (_tabButtons.Length == 0 || !_tabLists)
                return;

            int i = 0;
            foreach (Button button in _tabButtons)
            {
                GameObject tab = _tabLists.GetChild(i).gameObject;

                _tabs.Add(button, (tab, i));
                button.onClick.AddListener(() => SelectTab(button));

                ++i;
            }

            if (_rememberLastTab)
                _allIndices.TryAdd(_dictionaryIndex, _defaultTab);

            Button selected;

            if (_dictionaryIndex is not null && _allIndices.TryGetValue(_dictionaryIndex, out int index))
                selected = _tabs.ElementAt(index).Key;
            else
                selected = _tabs.ElementAt(_defaultTab).Key;

            _currentTabIndex = _tabs[selected].index;
        }

        protected override void Start()
        {
            SelectTab(_tabs.ElementAt(_currentTabIndex).Key);
        }

        private void Update()
        {
            if (_selectionMarker)
            {
                RectTransform rect = _tabs.ElementAt(_currentTabIndex).Key.GetComponent<RectTransform>();

                _selectionMarker.position = Vector3.Lerp(_selectionMarker.position, rect.position, Time.deltaTime * _selectionMarkerSpeed);
                _selectionMarker.sizeDelta = Vector3.Lerp(_selectionMarker.sizeDelta, rect.sizeDelta, Time.deltaTime * _selectionMarkerSpeed);
            }
        }

        public void SelectTab(Button tab)
        {
            _onTabDeselection.Invoke(_tabs.ElementAt(_currentTabIndex).Key);

            foreach ((GameObject gameObject, int index) otherTab in _tabs.Values)
            {
                otherTab.gameObject.SetActive(false);
            }
            _currentTabIndex = _tabs[tab].index;
            _tabs[tab].gameObject.SetActive(true);

            if (_rememberLastTab)
                _allIndices[_dictionaryIndex] = _currentTabIndex;

            _onTabSelection.Invoke(tab);
        }

        public void TabDirection(int direction)
        {
            if (!_cycle && (_currentTabIndex + direction >= _tabs.Count || _currentTabIndex - direction < 0))
                return;

            for (int i = 0; i < Mathf.Abs(direction); i++)
            {
                if (direction < 0)
                    TabLeft();
                else if (direction > 0)
                    TabRight();
            }
        }

        public void TabLeft()
        {
            if (!_cycle && _currentTabIndex - 1 < 0)
                return;

            _onTabDeselection.Invoke(_tabs.ElementAt(_currentTabIndex).Key);

            --_currentTabIndex;
            if (_currentTabIndex < 0)
                _currentTabIndex = _tabs.Count - 1;

            SelectTab(_tabs.ElementAt(_currentTabIndex).Key);
        }

        public void TabRight()
        {
            if (!_cycle && _currentTabIndex + 1 >= _tabs.Count)
                return;

            _onTabDeselection.Invoke(_tabs.ElementAt(_currentTabIndex).Key);

            ++_currentTabIndex;
            if (_currentTabIndex >= _tabs.Count)
                _currentTabIndex = 0;

            SelectTab(_tabs.ElementAt(_currentTabIndex).Key);
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Tab System - TextMeshPro", priority = 30)]
        private static void Create_Internal(UnityEditor.MenuCommand cmd)
        {
            GameObject obj = new("Tab System");

            UnityEditor.GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            UnityEditor.Selection.activeGameObject = obj;

            obj.AddComponent<TabSystem>();

            GameObject tabButtons = new("Tab Buttons");
            tabButtons.transform.SetParent(obj.transform);

            GameObject tabs = new("Tabs");
            tabs.transform.SetParent(obj.transform);
        }

#endif
    }
}
