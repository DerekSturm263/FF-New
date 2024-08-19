using GameResources.Camera;
using Quantum;
using UnityEngine;

public class SelectCameraDirection : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _left;
    [SerializeField] private CameraSettingsAsset _right;

    private EntityViewUpdater _entityViewUpdater;

    private CameraController _controller;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        _controller = GetComponent<CameraController>();

        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: e =>
        {
            if (_entityViewUpdater.GetView(e.Player).gameObject == transform.parent.gameObject)
            {
                if (e.Direction == -1)
                    _controller.SetCameraSettingsWithoutNotify(_left);
                else
                    _controller.SetCameraSettingsWithoutNotify(_right);
            }
        });
    }
}
