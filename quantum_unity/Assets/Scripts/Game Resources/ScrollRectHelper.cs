using Extensions.Components.Input;
using Extensions.Miscellaneous;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect), typeof(InputEvent))]
public class ScrollRectHelper : MonoBehaviour
{
    [SerializeField] private Helper.Direction _direction;
    [SerializeField] private float _scrollSpeed = -2000;
    [SerializeField] private float _lerpSpeed = 0.5f;

    private RectTransform _items;
    private ScrollRect _scrollRect;
    private RectTransform _viewRect;
    private RectTransform _contentRect;

    private Vector2 _normalizedScrollAmount;
    private float _defaultDecelerationRate;

    private void Reset()
    {
        InputEvent inputEvent = GetComponent<InputEvent>();
        inputEvent.SetButton(Resources.Load<InputButton>("Scriptable Objects/Input Buttons/Multi-Directional Scroll"));

#if UNITY_EDITOR

        UnityEditor.Events.UnityEventTools.AddPersistentListener(inputEvent.OnVector2Action, Scroll);

#endif
    }

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _viewRect = _scrollRect.viewport.GetComponent<RectTransform>();
        
        _items = _scrollRect.content;
        _contentRect = _items.GetComponent<RectTransform>();

        _defaultDecelerationRate = _scrollRect.decelerationRate;
    }

    private void Update()
    {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
        ScrollToItem(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
    }

    public void ScrollToItem(RectTransform itemRect)
    {
        RectTransform parentRect = itemRect.parent.GetComponent<RectTransform>();

        Vector2[] itemPos = GetPositionsInViewRect(itemRect);
        bool isContained = _viewRect.rect.Contains(itemPos[0]) && _viewRect.rect.Contains(itemPos[1]);

        if (!isContained)
        {
            _normalizedScrollAmount = default;
            if (itemPos[0].y < _viewRect.rect.yMin)
                _normalizedScrollAmount.y = -1;
            else if (itemPos[1].y > _viewRect.rect.yMax)
                _normalizedScrollAmount.y = 1;

            if (itemPos[0].x < _viewRect.rect.xMin)
                _normalizedScrollAmount.x = -1;
            else if (itemPos[1].x  > _viewRect.rect.xMax)
                _normalizedScrollAmount.x = 1;

            Scroll(_normalizedScrollAmount * _lerpSpeed);
        }
    }

    public void ScrollOverTime()
    {
        Vector2 newScrollPos = _scrollRect.normalizedPosition;

        if (_direction.HasFlag(Helper.Direction.Horizontal))
            newScrollPos.x = _normalizedScrollAmount.x;
        if (_direction.HasFlag(Helper.Direction.Vertical))
            newScrollPos.y = 1 - Mathf.Abs(_normalizedScrollAmount.y);

        _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, newScrollPos, Time.deltaTime * _lerpSpeed);
    }

    public void Scroll(Vector2 amount)
    {
        _scrollRect.velocity = amount * _scrollSpeed;
    }

    public Vector2[] GetPositionsInViewRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        return new Vector2[]
        {
            _viewRect.InverseTransformPoint(corners[0]),
            _viewRect.InverseTransformPoint(corners[2])
        };
    }

    public Vector2[] GetPositionsInContentRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        return new Vector2[]
        {
            _contentRect.InverseTransformPoint(corners[0]),
            _contentRect.InverseTransformPoint(corners[2])
        };
    }
}
