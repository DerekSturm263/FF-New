using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

        protected abstract Types.Tuple<string, Sprite> GetInfo(T item);
        public override void UpdateDisplay(T item)
        {
            Types.Tuple<string, Sprite> values = GetInfo(item);

            _component.Item1.Invoke(values.Item1);
            _component.Item2.Invoke(values.Item2);
        }
    }
}
