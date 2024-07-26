using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectHelper : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private LayoutGroup _items;

    private ScrollRect _scrollRect;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        foreach (EventTrigger eventTrigger in _items.GetComponentsInChildren<EventTrigger>())
        {
            EventTrigger.Entry select = eventTrigger.triggers.FirstOrDefault(item => item.eventID == EventTriggerType.Select);
            select?.callback.AddListener(eventData => ScrollToItem(eventData.selectedObject));
        }
    }

    public void Scroll(Vector2 amount)
    {
        _scrollRect.velocity = amount * _scrollSpeed;

        //EventSystem.current.SetSelectedGameObject();
    }

    public void ScrollToItem(GameObject item)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();
        RectTransform containerRect = _items.GetComponent<RectTransform>();

        Vector2 normalized = itemRect.anchoredPosition / containerRect.rect.size;

        _scrollRect.normalizedPosition = normalized;
    }
}
