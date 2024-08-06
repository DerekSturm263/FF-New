using ExitGames.Client.Photon;
using Extensions.Components.Input;
using Extensions.Miscellaneous;
using System.Collections.Generic;
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

    private Canvas _canvas;
    private GraphicRaycaster _graphicRaycaster;

    private Vector2 _normalizedScrollAmount;
    private Vector2 _oldSelectedPos;

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

        _canvas = GetComponentInParent<Canvas>();
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
    }

    private void Update()
    {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
            ScrollToItem(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
    }

    public void ScrollToItem(RectTransform itemRect)
    {
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

            ScrollInternal(_normalizedScrollAmount * _lerpSpeed, false);
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

    private Vector2 _oldAmount;

    public void Scroll(Vector2 amount)
    {
        if (_oldAmount == Vector2.zero && amount != Vector2.zero && EventSystem.current && EventSystem.current.currentSelectedGameObject)
        {
            _oldSelectedPos = GetScreenPos(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
        }

        _oldAmount = amount;
        if (amount == Vector2.zero)
            return;

        ScrollInternal(amount, true);
    }

    public Vector2 GetScreenPos(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Vector2 topLeft = corners[0];
        Vector2 bottomRight = new(corners[2].x, Screen.height - corners[2].y);

        Debug.Log(corners[2]);

        return (topLeft + bottomRight) / 2;
    }

    private void ScrollInternal(Vector2 amount, bool overrideSelection)
    {
        _scrollRect.velocity = amount * _scrollSpeed;

        if (overrideSelection)
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = _oldSelectedPos
            };

            List<RaycastResult> results = new();
            _graphicRaycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Selectable>())
                {
                    EventSystem.current.SetSelectedGameObject(result.gameObject);
                }
            }
        }
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
