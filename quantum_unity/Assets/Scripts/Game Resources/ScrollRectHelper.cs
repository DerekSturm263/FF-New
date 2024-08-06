using Extensions.Components.Input;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect), typeof(InputEvent))]
public class ScrollRectHelper : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = -2000;
    private RectTransform _items;

    private ScrollRect _scrollRect;
    private RectTransform _viewRect;
    private RectTransform _contentRect;

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
    }

    private void Start()
    {
        foreach (EventTrigger eventTrigger in _items.GetComponentsInChildren<EventTrigger>())
        {
            EventTrigger.Entry select = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Select);
            select?.callback.AddListener(ScrollToItem);
        }
    }

    public void Scroll(Vector2 amount)
    {
        //_scrollRect.velocity = amount * _scrollSpeed;
    }

    public void ScrollToItem(BaseEventData eventData)
    {
        /*RectTransform itemRect = eventData.selectedObject.GetComponent<RectTransform>();

        Vector2[] itemPos = GetPositionsInViewRect(itemRect);
        bool doScroll = !(_viewRect.rect.Contains(itemPos[0]) && _viewRect.rect.Contains(itemPos[1]));

        if (doScroll)
        {
            Vector3[] contentCorners = new Vector3[4];
            _contentRect.GetWorldCorners(contentCorners);

            Vector2[] itemContentPos = GetPositionsInContentRect(itemRect);

            Debug.Log(itemContentPos[0]);
            Debug.Log(contentCorners[0]);
            Debug.Log(contentCorners[2]);
            Debug.Log(contentCorners[0] - contentCorners[2]);

            _scrollRect.normalizedPosition = itemContentPos[0] / (contentCorners[0] - contentCorners[2]);
        }*/
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
