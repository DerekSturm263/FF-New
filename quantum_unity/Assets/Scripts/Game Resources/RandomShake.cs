using Extensions.Types;
using UnityEngine;
using UnityEngine.EventSystems;

public class RandomShake : UIBehaviour
{
    [SerializeField] private Range<float> _range;
    [SerializeField] private float _frequency;

    private RectTransform _rect;

    protected override void Awake()
    {
        _rect = GetComponent<RectTransform>();
        InvokeRepeating(nameof(UpdateTransform), _frequency, _frequency);
    }

    private void UpdateTransform()
    {
        Vector2 oldPos = _rect.anchoredPosition;
        oldPos.x = Random.Range(_range.Min, _range.Max);

        _rect.anchoredPosition = oldPos;
    }
}
