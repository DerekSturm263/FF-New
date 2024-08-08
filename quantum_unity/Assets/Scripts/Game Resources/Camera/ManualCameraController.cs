using Extensions.Miscellaneous;
using GameResources.UI.Popup;
using System;
using System.IO;
using UnityEngine;

public class ManualCameraController : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _settings;
    public void SetCameraSettings(CameraSettingsAsset settings) => _settings = settings;

    [SerializeField] private Camera _cam;

    private Controls _controls;

    private void Awake()
    {
        _controls = new();
    }

    private void Update()
    {
        Move(_controls.Camera.Move.ReadValue<Vector2>());
        Orbit(_controls.Camera.Orbit.ReadValue<Vector2>());

        Zoom(_controls.Camera.Zoom.ReadValue<float>());
        Tilt(_controls.Camera.Tilt.ReadValue<float>());
    }

    private void Move(Vector2 input)
    {
        if (input.magnitude < 0.1f)
            return;

        Vector3 direction = input;

        float dt = Time.unscaledDeltaTime;

        transform.Translate(direction * (dt * _settings.Settings.TranslationSpeed.AsFloat), Space.Self);
    }

    private void Orbit(Vector2 input)
    {
        if (input.magnitude < 0.1f)
            return;

        Vector3 rotation = new(-input.y, input.x, 0);

        float dt = Time.unscaledDeltaTime;

        transform.Rotate(rotation * (dt * _settings.Settings.RotationSpeed.AsFloat), Space.Self);
    }

    private void Zoom(float input)
    {
        Vector3 direction = new(0, 0, input);

        float dt = Time.unscaledDeltaTime;

        transform.Translate(direction * (dt * _settings.Settings.ZoomSpeed.AsFloat), Space.Self);
    }

    private void Tilt(float input)
    {
        Vector3 rotation = new(0, 0, -input);

        float dt = Time.unscaledDeltaTime;

        transform.Rotate(rotation * (dt * _settings.Settings.RotationSpeed.AsFloat), Space.Self);
    }

    public void Snap()
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/SaveData/Media/Pictures"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/SaveData/Media/Pictures");
        }
        
        _cam.RenderToScreenshot($"{Application.persistentDataPath}/SaveData/Media/Pictures/{DateTime.Now.ToFileTime()}.png", Helper.ImageType.PNG);

        ToastController.Instance.Spawn("Picture taken");
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}
