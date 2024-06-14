using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Auto Match Size", 13)]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class AutoMatchSize : UIBehaviour
    {
        [System.Flags]
        public enum Direction
        {
            Horizontal = 1 << 0,
            Vertical = 1 << 1
        }

        [SerializeField] private RectOffset _padding;
        [SerializeField] private Direction _direction;

        private RectTransform rectTransform;

        protected override void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (rectTransform.hasChanged)
            {
                Vector2 sizeDelta = rectTransform.sizeDelta;

                if (_direction.HasFlag(Direction.Horizontal))
                {
                    RectTransform child = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
                    sizeDelta.x = Mathf.Abs(child.anchoredPosition.x) + child.sizeDelta.x;
                }

                if (_direction.HasFlag(Direction.Vertical))
                {
                    RectTransform child = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
                    sizeDelta.y = Mathf.Abs(child.anchoredPosition.y) + child.sizeDelta.y;
                }

                rectTransform.sizeDelta = sizeDelta + new Vector2(_padding.left + _padding.right, _padding.bottom + _padding.top);
            }
        }
    }
}
