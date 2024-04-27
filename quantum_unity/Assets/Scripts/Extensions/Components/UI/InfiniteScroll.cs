using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _min;
    [SerializeField] private float _max;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rect.anchoredPosition += new Vector2(_speed * Time.deltaTime, 0);

        if (_rect.anchoredPosition.x < _min)
            _rect.anchoredPosition = new(_max, _rect.anchoredPosition.y);
    }
}
