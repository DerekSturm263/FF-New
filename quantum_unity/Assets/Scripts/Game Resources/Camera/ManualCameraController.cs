using UnityEngine;
using UnityEngine.InputSystem;

public class ManualCameraController : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _settings;
    public void SetCameraSettings(CameraSettingsAsset settings) => _settings = settings;

    private Camera _cam;
    public Camera Cam => _cam;

    private Controls _controls;

    private void Awake()
    {
        _controls = new();

        _controls.Camera.Move.performed += Move;
        _controls.Camera.Orbit.performed += Orbit;
        _controls.Camera.Zoom.performed += Zoom;
        _controls.Camera.Tilt.performed += Tilt;
        _controls.Camera.Snap.performed += Snap;
    }

    private void Move(InputAction.CallbackContext ctx)
    {

    }

    private void Orbit(InputAction.CallbackContext ctx)
    {

    }

    private void Zoom(InputAction.CallbackContext ctx)
    {

    }

    private void Tilt(InputAction.CallbackContext ctx)
    {

    }

    private void Snap(InputAction.CallbackContext ctx)
    {

    }
}
