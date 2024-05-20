using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _length;
    [SerializeField] private float _speed;
    [SerializeField] private float _intensity;

    private Light _light;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        _light.intensity = _curve.Evaluate(Mathf.Repeat(Time.time * _speed, _length)) * _intensity;
    }
}
