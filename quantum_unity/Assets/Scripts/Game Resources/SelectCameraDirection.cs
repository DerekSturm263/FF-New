using GameResources.Camera;
using UnityEngine;

public class SelectCameraDirection : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _left;
    [SerializeField] private CameraSettingsAsset _right;

    private CameraController _controller;

    private void Awake()
    {
        _controller = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (transform.parent.localScale.x == 1)
            _controller.SetCameraSettingsWithoutNotify(_right);
        else
            _controller.SetCameraSettingsWithoutNotify(_left);
    }
}
