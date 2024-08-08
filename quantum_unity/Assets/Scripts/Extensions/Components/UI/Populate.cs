using GameResources.UI.Popup;
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
            LazyWithParent,
            Never
        }

        [SerializeField] protected GameObject _button;

        [SerializeField] protected RectTransform _checkmark;
        protected RectTransform _checkmarkInstance;
        [SerializeField] protected TMPro.TMP_Text _sectionHeader;

        [SerializeField] protected Sprite _background;
        [SerializeField] protected Color _iconColor = Color.white;
        [SerializeField] protected Color _backgroundColor = Color.black;

        [SerializeField] protected LoadType _loadingType = LoadType.LazyWithParent;
        public LoadType LoadingType => _loadingType;

        [SerializeField] protected GameObject _parent;
        public GameObject Parent => _parent;

        [SerializeField] protected UnityEvent _onIfEmpty;
        [SerializeField] protected UnityEvent _onIfNotEmpty;

        public virtual List<string> AllFilterModeNames => new();
        public virtual List<string> AllGroupModeNames => new();
        public virtual List<string> AllSortModeNames => new();

        public abstract void SetFilterModeAtIndex(int index);
        public abstract void SetGroupModeAtIndex(int index);
        public abstract void SetSortModeAtIndex(int index);

        public abstract int GetCurrentFilterModeIndex();
        public abstract int GetCurrentGroupModeIndex();
        public abstract int GetCurrentSortModeIndex();

        public void Refresh()
        {
            DestroyList();
            GenerateList();
        }

        public abstract void GenerateList();
        public abstract void DestroyList();

        public abstract void EnableOrDisableItems();
        public abstract void EnableAllButtonNavigation(bool enabled);

        public abstract int Count();
    }

    public abstract class Populate<T> : PopulateBase
    {
        public enum SortDirection
        {
            Ascending = -1,
            Descending = 1
        }

        private static Populate<T> _instance;
        public static Populate<T> Instance => _instance;

        [SerializeField] protected UnityEvent<T> _onButtonSpawn;
        public void SubscribeOnButtonSpawn(UnityAction<T> action) => _onButtonSpawn.AddListener(action);

        [SerializeField] protected UnityEvent<T> _onButtonHover;
        public void SubscribeOnButtonHover(UnityAction<T> action)
        {
            _onButtonHover ??= new();
            _onButtonHover.AddListener(action);
        }

        [SerializeField] protected UnityEvent<T> _onButtonClick;
        public void SubscribeOnButtonClick(UnityAction<T> action)
        {
            _onButtonClick ??= new();
            _onButtonClick.AddListener(action);
        }

        [SerializeField] protected UnityEvent<T> _onButtonClickError;
        public void SubscribeOnButtonClickError(UnityAction<T> action) => _onButtonClickError.AddListener(action);

        [SerializeField] protected UnityEvent<T, Quantum.FighterIndex> _onButtonClickMultiplayer;
        public void SubscribeOnButtonClickMultiplayer(UnityAction<T, Quantum.FighterIndex> action) => _onButtonClickMultiplayer.AddListener(action);

        [SerializeField] protected UnityEvent<T> _onButtonDeselect;
        public void SubscribeOnButtonDeselect(UnityAction<T> action) => _onButtonDeselect.AddListener(action);

        [SerializeField] protected UnityEvent<T, int> _onIncrementalIncrementDecrement;
        public void SubscribeIncrementalIncrementDecrement(UnityAction<T, int> action) => _onIncrementalIncrementDecrement.AddListener(action);

        [SerializeField] protected TMPro.TMP_InputField _searchbar;

        [SerializeField] protected bool _helpButton = true;
        [SerializeField] protected Button _searchOptions;

        [SerializeField] protected bool _reloadOnEachEnable;

        protected Dictionary<T, GameObject> _itemsToButtons = new();
        protected List<GameObject> _headers = new();

        protected Dictionary<string, Predicate<T>> _allFilterModes;
        public override List<string> AllFilterModeNames => _allFilterModes.Keys.ToList();

        protected Dictionary<string, Func<T, (string, object)>> _allGroupModes;
        public override List<string> AllGroupModeNames => _allGroupModes.Keys.ToList();

        protected Dictionary<string, Comparison<T>> _allSortModes;
        public override List<string> AllSortModeNames => _allSortModes.Keys.ToList();

        private Predicate<T> _currentFilterMode;
        public override void SetFilterModeAtIndex(int index)
        {
            _currentFilterMode = _allFilterModes.ElementAt(index).Value;
            FilterList();
        }
        public override int GetCurrentFilterModeIndex() => _allFilterModes.Values.ToList().IndexOf(_currentFilterMode);

        private Func<T, (string, object)> _currentGroupMode;
        public override void SetGroupModeAtIndex(int index)
        {
            _currentGroupMode = _allGroupModes.ElementAt(index).Value;
            SortList(GroupList());
        }
        public override int GetCurrentGroupModeIndex() => _allGroupModes.Values.ToList().IndexOf(_currentGroupMode);

        private Comparison<T> _currentSortMode;
        public override void SetSortModeAtIndex(int index)
        {
            _currentSortMode = _allSortModes.ElementAt(index).Value;
            SortList(GroupList());
        }
        public override int GetCurrentSortModeIndex() => _allSortModes.Values.ToList().IndexOf(_currentSortMode);

        private SortDirection _currentSortDirection = SortDirection.Descending;

        protected override void Awake()
        {
            _instance = this;

            if (_checkmark)
                _checkmarkInstance = Instantiate(_checkmark);

            if (_searchbar)
            {
                _searchbar.onValueChanged.AddListener(value =>
                {
                    FilterList(item => Name(item).Contains(value, StringComparison.CurrentCultureIgnoreCase));
                });
            }

            if (_searchOptions)
            {
                _searchOptions.onClick.AddListener(() => SearchOptionsController.Instance.Spawn(this));
            }

            base.Awake();
        }

        protected override void OnEnable()
        {
            if (_loadingType == LoadType.Never)
                return;

            if (_loadingType == LoadType.Lazy)
            {
                GenerateList();
            }

            foreach (var kvp in _itemsToButtons)
            {
                ClearEvents(kvp.Value);
                SetEvents(kvp.Value, kvp.Key);
            }

            _allFilterModes ??= GetAllFilterModes();
            _allGroupModes ??= GetAllGroupModes();
            _allSortModes ??= GetAllSortModes();

            _currentFilterMode = GetDefaultFilterMode();
            _currentGroupMode = GetDefaultGroupMode();
            _currentSortMode = GetDefaultSortMode();

            FilterList();
            SortList(GroupList());

            EnableOrDisableItems();

            if (_checkmarkInstance)
            {
                var firstEquipped = _itemsToButtons.FirstOrDefault(item => IsEquipped(item.Key));

                if (!firstEquipped.Equals(default(KeyValuePair<T, GameObject>)))
                    ParentCheckmark(firstEquipped.Value);
                else
                    ParentCheckmark(_itemsToButtons.FirstOrDefault(item => IsNone(item.Key)).Value);
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

        public void FilterList(params Func<T, bool>[] extraFunctions)
        {
            List<T> filtered = _itemsToButtons.Keys.Where(item => _currentFilterMode.Invoke(item) || IsNone(item)).ToList();
            foreach (var func in extraFunctions)
            {
                filtered = filtered.Where(func).ToList();
            }

            if (extraFunctions.Length > 0)
            {
                var element = filtered.FirstOrDefault(item => IsNone(item));

                if (element is not null)
                    filtered.Remove(element);
            }

            foreach (var kvp in _itemsToButtons)
            {
                kvp.Value.SetActive(filtered.Contains(kvp.Key));
            }

            if (filtered.Count() == 0)
                _onIfEmpty?.Invoke();
            else
                _onIfNotEmpty?.Invoke();
        }

        public IEnumerable<IGrouping<(string, object), T>> GroupList()
        {
            if (!_sectionHeader)
                return _itemsToButtons.Keys.GroupBy(item => ("All", 0 as object));

            return _itemsToButtons.Keys.GroupBy(_currentGroupMode);
        }

        public void SortList(IEnumerable<IGrouping<(string, object), T>> groups)
        {
            for (int i = 0; i < _headers.Count; ++i)
                Destroy(_headers[i]);

            _headers.Clear();

            int offset = 0;
            foreach (var group in groups)
            {
                if (_sectionHeader)
                {
                    TMPro.TMP_Text header = Instantiate(_sectionHeader, transform);
                    header.margin = new(20, 0, 0, 0);

                    header.SetText(group.Key.Item1.ToString());
                    header.transform.SetSiblingIndex(offset);

                    _headers.Add(header.gameObject);
                }

                List<T> sorted = group.ToList();
                sorted.Sort((lhs, rhs) => _currentSortMode.Invoke(lhs, rhs) * (int)_currentSortDirection);

                int headerOffset = _sectionHeader ? 1 : 0;
                for (int i = 0; i < sorted.Count; ++i)
                {
                    _itemsToButtons[sorted[i]].transform.SetSiblingIndex(i + offset + headerOffset);
                    ++offset;
                }
            }

            var none = _itemsToButtons.FirstOrDefault(item => IsNone(item.Key));
            if (none.Key is not null && none.Value)
                none.Value.transform.SetSiblingIndex(_headers.Count > 0 ? 1 : 0);

            var top = gameObject.FindChildWithTag("Top", false);
            if (top)
                top.transform.SetAsFirstSibling();

            Button firstButton = null;
            for (int i = 0; i < transform.childCount; ++i)
            {
                if (transform.GetChild(i).TryGetComponent<Button>(out var button))
                {
                    firstButton = button;
                    break;
                }
            }

            if (_helpButton)
            {
                if (_searchOptions && firstButton)
                {
                    _searchOptions.navigation = new()
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnLeft = _searchOptions.navigation.selectOnLeft,
                        selectOnDown = firstButton
                    };
                }
            }
            else
            {
                _searchOptions.navigation = new Navigation() { mode = Navigation.Mode.Automatic };
            }
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
        protected virtual Predicate<T> GetDefaultFilterMode() => _allFilterModes.ElementAt(0).Value;

        protected abstract Dictionary<string, Func<T, (string, object)>> GetAllGroupModes();
        protected virtual Func<T, (string, object)> GetDefaultGroupMode() => _allGroupModes.ElementAt(0).Value;

        protected abstract Dictionary<string, Comparison<T>> GetAllSortModes();
        protected virtual Comparison<T> GetDefaultSortMode() => _allSortModes.ElementAt(0).Value;

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
        public virtual void SetEvents(GameObject buttonObj, T item)
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
        public void ClearEvents(GameObject buttonObj)
        {
            Button button = buttonObj.GetComponentInChildren<Button>();
            button?.onClick.RemoveAllListeners();

            MultiplayerButton multiplayerButton = buttonObj.GetComponentInChildren<MultiplayerButton>();
            multiplayerButton?.onClick.RemoveAllListeners();

            DragAndDropItem dragAndDrop = buttonObj.GetComponentInChildren<DragAndDropItem>();
            dragAndDrop?.SetValue(null);

            Incremental incremental = buttonObj.GetComponentInChildren<Incremental>();
            incremental?.OnIncrementDecrement.RemoveAllListeners();
        }

        private void ParentCheckmark(GameObject parent)
        {
            _checkmarkInstance.SetParent(parent.FindChildWithTag("Offset", true).transform);
            _checkmarkInstance.localScale = Vector3.one;

            _checkmarkInstance.anchoredPosition = new(20, -30);
            _checkmarkInstance.anchorMin = new(0, 1);
            _checkmarkInstance.anchorMax = new(0, 1);
        }

        public bool TryGetButtonFromItem(T item, out GameObject button)
        {
            button = null;

            if (_itemsToButtons is null || item is null || !_itemsToButtons.TryGetValue(item, out button))
                return false;

            return true;
        }

        public bool ContainsItem(T item) => _itemsToButtons.ContainsKey(item);
        public override int Count() => _itemsToButtons.Count;
    }
}
