using Extensions.Miscellaneous;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Display", 80)]
    public abstract class DisplayBase : UIBehaviour
    {
        [SerializeField] private bool _evaluateOnEnable;

        protected override void OnEnable()
        {
            if (_evaluateOnEnable)
                UpdateDisplayOnEnable();
        }

        public abstract void UpdateDisplayOnEnable();
    }

    public abstract class Display<T, TComponent> : DisplayBase
    {
        [SerializeField] protected TComponent _component;

        public override void UpdateDisplayOnEnable() => UpdateDisplay();

        protected abstract T GetValue();

        public abstract void UpdateDisplay(T item);
        public void UpdateDisplay() => UpdateDisplay(GetValue());
    }

    public abstract class DisplayText<T> : Display<T, UnityEvent<string>>
    {
        [SerializeField][TextArea] protected string _format = "{0}";

        protected abstract string GetInfo(T item);
        public override void UpdateDisplay(T item) => _component.Invoke(string.Format(_format, GetInfo(item)));
    }

    public abstract class DisplayT<T> : Display<T, UnityEvent<T>>
    {
        protected abstract string GetInfo(T item);
        public override void UpdateDisplay(T item) => _component.Invoke(item);
    }

    public abstract class DisplayImage<T> : Display<T, UnityEvent<Sprite>>
    {
        protected abstract Sprite GetInfo(T item);
        public override void UpdateDisplay(T item) => _component.Invoke(GetInfo(item));
    }

    public abstract class DisplayTextAndImage<T> : Display<T, Types.Tuple<UnityEvent<string>, UnityEvent<Sprite>>>
    {
        [SerializeField][TextArea] protected string _format = "{0}";
        [SerializeField] private bool _resizeIfEmpty = true;

        protected abstract Types.Tuple<string, Sprite> GetInfo(T item);
        public override void UpdateDisplay(T item)
        {
            Types.Tuple<string, Sprite> values = GetInfo(item);

            _component.Item1.Invoke(values.Item1);

            TMPro.TMP_Text text = GetComponentInChildren<TMPro.TMP_Text>();
            Image image = gameObject.FindChildWithTag("Icon", true)?.GetComponent<Image>();

            image?.gameObject.SetActive(values.Item2);

            if (!values.Item2)
            {
                if (_resizeIfEmpty && text)
                {
                    RectTransform rect = text.rectTransform;

                    rect.anchorMin = new(0, rect.anchorMin.y);
                    rect.anchoredPosition = new(0, rect.anchoredPosition.y);
                    rect.sizeDelta = new(-90, rect.sizeDelta.y);
                }
            }
            else
            {
                if (_resizeIfEmpty && text)
                {
                    RectTransform rect = text.rectTransform;

                    rect.anchorMin = new(0.35f, rect.anchorMin.y);
                    rect.anchoredPosition = new(-22.5f, rect.anchoredPosition.y);
                    rect.sizeDelta = new(-45, rect.sizeDelta.y);
                }

                _component.Item2.Invoke(values.Item2);
            }
        }
    }
}
