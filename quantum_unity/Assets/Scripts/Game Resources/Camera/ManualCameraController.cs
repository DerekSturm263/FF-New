using UnityEngine;

public class ManualCameraController : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _settings;
    public void SetCameraSettings(CameraSettingsAsset settings) => _settings = settings;

    private Camera _cam;
    public Camera Cam => _cam;

    private void LateUpdate()
    {
        if (!_settings)
            return;


    }
}
