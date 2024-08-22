using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private TMPro.TMP_Text _label;

    private RectTransform _rect;
    private FighterIndex _playerIndex;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {

    }

    public void Initialize(FighterIndex playerIndex)
    {
        _playerIndex = playerIndex;

        //_arrow.color = playerIndex.GetLightColor();
        _label.SetText($"P{playerIndex.GlobalNoBots + 1}");
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
