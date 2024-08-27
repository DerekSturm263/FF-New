using UnityEngine;

public class FollowMainCamera : MonoBehaviour
{
    private Transform _target;

    private void Start()
    {
        _target = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(_target.transform.position, _target.transform.rotation);
    }
}
