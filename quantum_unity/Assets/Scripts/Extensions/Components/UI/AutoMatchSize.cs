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

        [SerializeField] private Direction _direction;
        [SerializeField] private Vector2 _padding;

        private RectTransform _rectTransform;
        private RectTransform _parentTransform;

        protected override void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentTransform = transform.parent.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_rectTransform.hasChanged)
            {
                Vector2 sizeDelta = _rectTransform.sizeDelta;

                if (_direction.HasFlag(Direction.Horizontal))
                {
                    float biggestX = 0;
                    float width = 0;

                    for (int i = transform.childCount - 1; i >= 0; --i)
                    {
                        RectTransform rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (!rect.gameObject.activeSelf)
                            continue;

                        float newX = Mathf.Abs(rect.anchoredPosition.x);
                        if (newX > biggestX)
                        {
                            biggestX = newX;
                            width = rect.sizeDelta.x;

                            break;
                        }
                    }

                    float newSize = biggestX - width / 2 + width;
                    sizeDelta.x = Mathf.Max(newSize + _padding.x, _parentTransform.rect.width);
                }

                if (_direction.HasFlag(Direction.Vertical))
                {
                    float biggestY = 0;
                    float height = 0;

                    for (int i = transform.childCount - 1; i >= 0; --i)
                    {
                        RectTransform rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (!rect.gameObject.activeSelf)
                            continue;

                        float newY = Mathf.Abs(rect.anchoredPosition.y);
                        if (newY > biggestY)
                        {
                            biggestY = newY;
                            height = rect.sizeDelta.y;

                            break;
                        }
                    }

                    float newSize = biggestY - height / 2 + height;
                    sizeDelta.y = Mathf.Max(newSize + _padding.y, _parentTransform.rect.height);
                }

                _rectTransform.sizeDelta = sizeDelta;
            }
        }
    }
}
