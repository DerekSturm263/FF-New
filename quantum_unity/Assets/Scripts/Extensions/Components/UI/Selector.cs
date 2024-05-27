using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private TMPro.TMP_Text _label;

    [SerializeField] private Color[] _colors;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Initialize(int playerNum)
    {
        _arrow.color = _colors[playerNum];
        _label.SetText($"P{playerNum + 1}");
    }

    public void ChildToSelected(GameObject selected)
    {
        _rect ??= GetComponent<RectTransform>();

        transform.SetParent(selected.transform);

        _rect.anchoredPosition = new(-30, 8);
        _rect.anchorMin = new(1, 1);
        _rect.anchorMax = new(1, 1);
    }
}
