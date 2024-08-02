using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Extensions.Miscellaneous.Helper;

namespace Extensions.Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class PopulateBase : UIBehaviour
    {
        public enum LoadType
        {
            Awake,
            Lazy,
            LazyWithParent
        }

        [SerializeField] protected GameObject _button;

        [SerializeField] protected RectTransform _checkmark;
        protected RectTransform _checkmarkInstance;

        [SerializeField] protected Sprite _background;
        [SerializeField] protected Color _iconColor = Color.white;
        [SerializeField] protected Color _backgroundColor = Color.black;

        [SerializeField] protected bool _includesNone;

        [SerializeField] protected LoadType _loadingType = LoadType.LazyWithParent;
        public LoadType LoadingType => _loadingType;

        [SerializeField] protected GameObject _parent;
        public GameObject Parent => _parent;

        [SerializeField] protected UnityEvent _onIfEmpty;
        [SerializeField] protected UnityEvent _onIfNotEmpty;

        public void Refresh()
        {
            DestroyList();
            GenerateList();
        }

        public abstract void GenerateList();
        public abstract void DestroyList();
        public abstract void FilterList();
        public abstract void SortList();
        public abstract void EnableOrDisableItems();
        public abstract void EnableAllButtonNavigation(bool enabled);

        public abstract int Count();
    }

    [RequireComponent(typeof(LayoutGroup))]
    public abstract class Populate<T> : PopulateBase
    {
        public enum SortDirection
        {
            Ascending = -1,
            Descending = 1
        }

        protected Dictionary<T, GameObject> _itemsToButtons = new();

        public bool TryGetButtonFromItem(T item, out GameObject button)
        {
            button = null;

            if (_itemsToButtons is null || item is null || !_itemsToButtons.TryGetValue(item, out button))
                return false;

            return true;
        }

        public bool ContainsItem(T item) => _itemsToButtons.ContainsKey(item);
        public override int Count() => _itemsToButtons.Count;

        [SerializeField] protected UnityEvent<T> _onButtonSpawn;
        [SerializeField] protected UnityEvent<T> _onButtonHover;
        [SerializeField] protected UnityEvent<T> _onButtonClick;
        [SerializeField] protected UnityEvent<T> _onButtonClickError;
        [SerializeField] protected UnityEvent<T, Quantum.FighterIndex> _onButtonClickMultiplayer;
        [SerializeField] protected UnityEvent<T> _onButtonDeselect;
        [SerializeField] protected UnityEvent<T, int> _onIncrementalIncrementDecrement;

        [SerializeField] private TMPro.TMP_Dropdown _filterDropdown;
        [SerializeField] private TMPro.TMP_Dropdown _sortDropdown;

        [SerializeField] protected bool _reloadOnEachEnable;

        protected Dictionary<string, Predicate<T>> _allFilterModes;
        protected Dictionary<string, Comparison<T>> _allSortModes;

        private Predicate<T> _currentFilterMode;
        private Comparison<T> _currentSortMode;
        private SortDirection _currentSortDirection = SortDirection.Descending;

        private static Populate<T> _instance;
        public static Populate<T> Instance => _instance;

        protected override void Awake()
        {
            _instance = this;

            if (_checkmark)
                _checkmarkInstance = Instantiate(_checkmark);

            if (_sortDropdown)
            {
                _sortDropdown.AddOptions(_allSortModes.Select(item => new TMPro.TMP_Dropdown.OptionData(item.Key)).ToList());
                _sortDropdown.onValueChanged.AddListener((value) =>
                {
                    _currentSortMode = _allSortModes.ElementAt(value).Value;
                    SortList();
                });
            }

            if (_filterDropdown)
            {
                _filterDropdown.AddOptions(_allFilterModes.Select(item => new TMPro.TMP_Dropdown.OptionData(item.Key)).ToList());
                _filterDropdown.onValueChanged.AddListener((value) =>
                {
                    _currentFilterMode = _allFilterModes.ElementAt(value).Value;
                    FilterList();
                });
            }

            base.Awake();
        }

        protected override void OnEnable()
        {
            if (_loadingType == LoadType.Lazy)
            {
                GenerateList();
            }

            EnableOrDisableItems();

            if (_checkmarkInstance)
            {
                ParentCheckmark(_itemsToButtons.First(item => IsEquipped(item.Key)).Value);
            }
        }

        protected override void OnDisable()
        {
            if (_reloadOnEachEnable)
            {
                DestroyList();
            }
        }

        public override void GenerateList()
        {
            if (_itemsToButtons.Count > 0)
                return;

            IEnumerable<T> items = LoadAll();

            foreach (T item in items)
            {
                AddItem(item);
            }

            FilterList();
            SortList();
        }

        public override void DestroyList()
        {
            List<T> keys = new(_itemsToButtons.Keys);

            foreach (T item in keys)
            {
                RemoveItem(item);
            }
        }

        public GameObject AddItem(T item)
        {
            GameObject buttonObj = Instantiate(_button, transform);

            Decorate(buttonObj, item);
            SetEvents(buttonObj, item);
            
            _onButtonSpawn.Invoke(item);
            _itemsToButtons.TryAdd(item, buttonObj);

            Debug.Log($"Loaded {typeof(T).Name}: {Name(item)}");
            return buttonObj;
        }

        public void RemoveItem(T item)
        {
            if (_itemsToButtons.TryGetValue(item, out GameObject button))
            {
                Destroy(button);

                Debug.Log($"Destroyed {typeof(T).Name}: {Name(item)}");
                _itemsToButtons.Remove(item);
            }
        }

        public override void FilterList()
        {
            _allFilterModes ??= GetAllFilterModes();
            _currentFilterMode ??= GetDefaultFilterMode();

            IEnumerable<T> filtered = _itemsToButtons.Keys.Where(_currentFilterMode.Invoke);

            foreach (var kvp in _itemsToButtons)
            {
                kvp.Value.SetActive(filtered.Contains(kvp.Key));
            }

            if (filtered.Count() == 0 || (filtered.Count() == 1 && _includesNone))
                _onIfEmpty.Invoke();
            else
                _onIfNotEmpty.Invoke();
        }

        public override void SortList()
        {
            _allSortModes ??= GetAllSortModes();
            _currentSortMode ??= GetDefaultSortMode();

            List<T> sorted = _itemsToButtons.Keys.ToList();
            sorted.Sort((lhs, rhs) => _currentSortMode.Invoke(lhs,rhs) * (int)_currentSortDirection);

            for (int i = 0; i < sorted.Count; ++i)
            {
                _itemsToButtons[sorted[i]].transform.SetSiblingIndex(i);
            }

            var none = _itemsToButtons.FirstOrDefault(item => IsNone(item.Key));
            if (none.Key is not null && none.Value)
                none.Value.transform.SetAsFirstSibling();

            var top = gameObject.FindChildWithTag("Top", false);
            if (top)
                top.transform.SetAsFirstSibling();
        }

        public override void EnableOrDisableItems()
        {
            foreach (var kvp in _itemsToButtons)
            {
                bool enabled = Enabled(kvp.Key);

                if (kvp.Value.TryGetComponent(out Button button))
                    button.interactable = enabled;
                else if (kvp.Value.TryGetComponent(out DragAndDropItem dragAndDrop))
                    dragAndDrop.interactable = enabled;
            }
        }

        public override void EnableAllButtonNavigation(bool enabled)
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                button.navigation = new() { mode = enabled ? Navigation.Mode.Automatic : Navigation.Mode.None };
            }
        }

        protected abstract IEnumerable<T> LoadAll();

        protected abstract string Name(T item);
        protected abstract string Description(T item);
        protected abstract Sprite Icon(T item);

        protected abstract Dictionary<string, Predicate<T>> GetAllFilterModes();
        protected abstract Predicate<T> GetDefaultFilterMode();

        protected abstract Dictionary<string, Comparison<T>> GetAllSortModes();
        protected abstract Comparison<T> GetDefaultSortMode();

        protected virtual bool Enabled(T item) => true;
        protected virtual bool GiveEvents(T item) => true;
        protected abstract bool IsEquipped(T item);
        protected abstract bool IsNone(T item);

        protected virtual void Decorate(GameObject buttonObj, T item)
        {
            string name = Name(item);

            buttonObj.name = $"{name} Button";

            TMPro.TMP_Text label = buttonObj.FindChildWithTag("Label", false)?.GetComponent<TMPro.TMP_Text>();
            if (label)
                label.SetText(name);

            TMPro.TMP_Text description = buttonObj.FindChildWithTag("Description", false)?.GetComponent<TMPro.TMP_Text>();
            if (description)
                description.SetText(Description(item));

            Image icon = buttonObj.FindChildWithTag("Icon", false)?.GetComponent<Image>();
            if (icon)
            {
                try
                {
                    icon.sprite = Icon(item);
                }
                catch { }

                if (!icon.sprite)
                    icon.enabled = false;
            }

            Image background = buttonObj.FindChildWithTag("Background", false)?.GetComponent<Image>();
            if (background)
            {
                Sprite backgroundSprite = _background;

                if (backgroundSprite)
                    background.sprite = backgroundSprite;

                //background.color = _backgroundColor;
            }

            GameObject color = buttonObj.FindChildWithTag("Color", false);
            if (color)
                color.GetComponent<Image>().color = _iconColor;
        }
        protected virtual void SetEvents(GameObject buttonObj, T item)
        {
            bool giveEvents = GiveEvents(item);

            Button button = buttonObj.GetComponentInChildren<Button>();
            MultiplayerButton multiplayerButton = buttonObj.GetComponentInChildren<MultiplayerButton>();
            DragAndDropItem dragAndDrop = buttonObj.GetComponentInChildren<DragAndDropItem>();
            Incremental incremental = buttonObj.GetComponentInChildren<Incremental>();

            if (giveEvents)
            {
                if (button)
                {
                    button.onClick.AddListener(() =>
                    {
                        _onButtonClick.Invoke(item);

                        if (_checkmarkInstance)
                        {
                            ParentCheckmark(button.gameObject);
                        }
                    });
                }

                if (multiplayerButton)
                    multiplayerButton.onClick.AddListener((playerNum) => _onButtonClickMultiplayer.Invoke(item, playerNum));

                if (dragAndDrop)
                    dragAndDrop.SetValue(item);

                if (incremental)
                    incremental.OnIncrementDecrement.AddListener((amount) => _onIncrementalIncrementDecrement.Invoke(item, amount));
            }
            else if (button)
            {
                button.onClick.AddListener(() => _onButtonClickError.Invoke(item));
            }

            EventTrigger eventTrigger = buttonObj.GetComponentInChildren<EventTrigger>();
            if (eventTrigger)
            {
                EventTrigger.Entry select = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Select);
                select?.callback.AddListener(_ => _onButtonHover.Invoke(item));

                EventTrigger.Entry deselect = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Deselect);
                deselect?.callback.AddListener(_ => _onButtonDeselect.Invoke(item));
            }
        }

        private void ParentCheckmark(GameObject parent)
        {
            _checkmarkInstance.SetParent(parent.FindChildWithTag("Offset", true).transform);
            _checkmarkInstance.localScale = Vector3.one;

            _checkmarkInstance.anchoredPosition = new(20, -30);
            _checkmarkInstance.anchorMin = new(0, 1);
            _checkmarkInstance.anchorMax = new(0, 1);
        }
    }
}
