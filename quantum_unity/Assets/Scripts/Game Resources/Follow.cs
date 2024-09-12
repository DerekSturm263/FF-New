using UnityEngine;

public class Follow : MonoBehaviour
{
    private Transform _target;
    private Vector3 _offset;

    public void SetTarget(Transform target, Vector3 offset)
    {
        _target = target;
        _offset = offset;
    }

    private void Start()
    {
        if (!_target)
            _target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(_target.transform.position + _offset, _target.transform.rotation);
    }
}
