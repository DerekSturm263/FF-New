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
    [DisallowMultipleComponent]
    public abstract class PopulateBase : UIBehaviour
    {
        public enum LoadType
        {
            Default,
            Custom
        }

        public enum LoadStage
        {
            Awake,
            Lazy
        }

        protected static Types.Dictionary<System.Type, Types.Dictionary<object, GameObject>> _itemsToButtons = new();

        [SerializeField] protected GameObject _button;
        [SerializeField] protected Color _color = Color.white;

        [SerializeField] protected LoadStage _loadingType = LoadStage.Awake;
        public LoadStage LoadingType => _loadingType;

        [SerializeField] protected float _sizeOffset;

        [SerializeField] protected UnityEvent _onIfEmpty;

        protected RectTransform _containerRect;
        protected ScrollRect _scrollRect;
        protected int _itemCount;

        protected bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        protected override void Awake()
        {
            _itemsToButtons = new();

            if (_loadingType == LoadStage.Lazy)
                LoadAllItems();

            _containerRect = GetComponent<RectTransform>();
            _scrollRect = GetComponentInParent<ScrollRect>();
        }

        public abstract void LoadAllItems();
        public abstract void RemoveFromList();

        protected override void OnEnable()
        {
            EnableOrDisableItems();
        }

        protected abstract void EnableOrDisableItems();

        public void ScrollListBy(Vector2 amount)
        {
            _scrollRect.velocity = amount * -1000;
        }

        public void ScrollListTo(Transform buttonTransform)
        {
            _scrollRect.verticalNormalizedPosition = 1 - (buttonTransform.GetSiblingIndex() / (float)_itemCount);
        }
    }

    [RequireComponent(typeof(LayoutGroup))]
    public abstract class Populate<T, TSorter> : PopulateBase
    {
        public static GameObject ButtonFromItem(T item)
        {
            if (_itemsToButtons is null || item is null || !_itemsToButtons[typeof(T)].TryGetValue(item, out GameObject value))
                return null;

            return value;
        }
        public static bool TryButtonFromItem(T item, out GameObject button)
        {
            button = null;

            if (_itemsToButtons is null || item is null || !_itemsToButtons[typeof(T)].TryGetValue(item, out button))
                return false;

            return true;
        }

        public static bool ContainsItem(T item) => _itemsToButtons[typeof(T)].ContainsKey(item);
        public static int CountAll<U>() => _itemsToButtons[typeof(U)].Count;

        [SerializeField] protected UnityEvent<T> _onButtonSpawn;
        [SerializeField] protected UnityEvent<T> _onButtonHover;
        [SerializeField] protected UnityEvent<T> _onButtonClick;
        [SerializeField] protected UnityEvent<T, int> _onButtonClickMultiplayer;
        [SerializeField] protected UnityEvent<T> _onButtonDeselect;

        public override void LoadAllItems()
        {
            _itemsToButtons.TryAdd(typeof(T), new());
            _itemCount = transform.childCount;

            _containerRect = GetComponent<RectTransform>();
            _scrollRect = GetComponentInParent<ScrollRect>();

            IEnumerable<T> items = LoadAll();
            items = items.OrderBy(item => Sort()(item)).ToList();

            foreach (T item in items)
                AddItem(item);

            if (_itemCount == 0)
                _onIfEmpty.Invoke();

            _isInitialized = true;
        }

        public override void RemoveFromList()
        {
            _itemsToButtons.Remove(typeof(T));
        }
        protected override void EnableOrDisableItems()
        {
            if (transform.childCount == 0 || _itemsToButtons is null)
                return;

            if (_itemsToButtons.ContainsKey(typeof(T)))
            {
                foreach (var kvp in _itemsToButtons[typeof(T)])
                {
                    bool enabled = Enabled((T)kvp.Key);

                    if (kvp.Value.TryGetComponent(out Button button))
                        button.interactable = enabled;
                    else if (kvp.Value.TryGetComponent(out DragAndDropItem dragAndDrop))
                        dragAndDrop.interactable = enabled;
                }
            }
        }

        protected virtual GameObject Button(T item) => _button;

        public GameObject AddItem(T item)
        {
            if (!DoSpawn(item))
                return null;

            GameObject buttonObj = Instantiate(Button(item), transform);

            Decorate(buttonObj, item);
            SetEvents(buttonObj, item);
            
            Debug.Log($"Loaded {typeof(T).Name}: {Name(item)}");
            _onButtonSpawn.Invoke(item);
            _itemsToButtons[typeof(T)].TryAdd(item, buttonObj);
            ++_itemCount;

            return buttonObj;
        }

        public void EnableAllButtonNavigation(bool enabled)
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                button.navigation = new() { mode = enabled ? Navigation.Mode.Automatic : Navigation.Mode.None };
            }
        }

        protected abstract string Name(T item);
        protected abstract Sprite Icon(T item);
        protected virtual Sprite Background(T item) => null;
        protected virtual Color[] ColorPalette(T item) => new Color[] { _color, _color, _color };
        protected virtual string Description(T item) => string.Empty;

        protected abstract IEnumerable<T> LoadAll();
        protected abstract System.Func<T, TSorter> Sort();

        protected virtual bool Enabled(T item) => true;
        protected virtual bool DoSpawn(T item) => true;

        protected virtual void Decorate(GameObject buttonObj, T item)
        {
            string name = Name(item);

            buttonObj.name = $"{name} Button";

            TMPro.TMP_Text label = buttonObj.FindChildWithTag("Label")?.GetComponent<TMPro.TMP_Text>();
            if (label)
                label.SetText(name);

            TMPro.TMP_Text description = buttonObj.FindChildWithTag("Description")?.GetComponent<TMPro.TMP_Text>();
            if (description)
                description.SetText(Description(item));

            Image icon = buttonObj.FindChildWithTag("Icon")?.GetComponent<Image>();
            if (icon)
            {
                icon.sprite = Icon(item);

                if (!icon.sprite)
                    icon.enabled = false;
            }

            Color[] colorPalette = ColorPalette(item);

            Image background = buttonObj.FindChildWithTag("Background")?.GetComponent<Image>();
            if (background)
            {
                Sprite backgroundSprite = Background(item);

                if (backgroundSprite)
                    background.sprite = backgroundSprite;

                background.color = colorPalette[2];
            }

            GameObject color = buttonObj.FindChildWithTag("Color");
            if (color)
                color.GetComponent<Image>().color = colorPalette[0];
        }

        protected virtual void SetEvents(GameObject buttonObj, T item)
        {
            Button button = buttonObj.GetComponentInChildren<Button>();
            if (button)
                button.onClick.AddListener(() => _onButtonClick.Invoke(item));

            MultiplayerButton multiplayerButton = buttonObj.GetComponentInChildren<MultiplayerButton>();
            if (multiplayerButton)
                multiplayerButton.onClick.AddListener((playerNum) => _onButtonClickMultiplayer.Invoke(item, playerNum));

            DragAndDropItem dragAndDrop = buttonObj.GetComponentInChildren<DragAndDropItem>();
            if (dragAndDrop)
                dragAndDrop.SetValue(item);

            EventTrigger eventTrigger = buttonObj.GetComponentInChildren<EventTrigger>();
            if (eventTrigger)
            {
                EventTrigger.Entry select = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Select);
                select?.callback.AddListener(_ => _onButtonHover.Invoke(item));

                EventTrigger.Entry deselect = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Deselect);
                deselect?.callback.AddListener(_ => _onButtonDeselect.Invoke(item));
            }
        }
    }
}
