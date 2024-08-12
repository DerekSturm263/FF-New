using Extensions.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using System;
using UnityEngine;
using static CustomAnimatorGraphAsset;

public class ManualCameraController : MonoBehaviour
{
    [SerializeField] private CameraSettingsAsset _settings;
    public void SetCameraSettings(CameraSettingsAsset settings) => _settings = settings;

    [SerializeField] private Camera _cam;

    [SerializeField] private RenderTexture _picture;

    private Controls _controls;

    private Vector3 _rotation;

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

        transform.rotation = Quaternion.Euler(_rotation);
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

        float dt = Time.unscaledDeltaTime;

        _rotation.x -= input.y * dt * _settings.Settings.RotationSpeed.AsFloat;
        _rotation.y += input.x * dt * _settings.Settings.RotationSpeed.AsFloat;
    }

    private void Zoom(float input)
    {
        Vector3 direction = new(0, 0, input);

        float dt = Time.unscaledDeltaTime;

        transform.Translate(direction * (dt * _settings.Settings.ZoomSpeed.AsFloat), Space.Self);
    }

    private void Tilt(float input)
    {
        float dt = Time.unscaledDeltaTime;

        _rotation.z -= input * dt * _settings.Settings.RotationSpeed.AsFloat;
    }

    public void Snap()
    {
        AssetGuid guid = AssetGuid.NewGuid();

        SerializableWrapper<Picture> picture = new(default, PictureController.GetPath(), DateTime.Now.ToUniversalTime().ToString("U"), "", guid, new string[] { }, new Extensions.Types.Tuple<string, string>[] { });
        picture.Save();

        picture.CreateIcon(_cam);
        picture.CreatePreview(_cam);

        RenderTextureDescriptor descriptor = new(_picture.width, _picture.height, RenderTextureFormat.ARGB32, 8, 0, RenderTextureReadWrite.sRGB);
        RenderTexture pictureTexture = new(descriptor);

        _cam.RenderToScreenshot($"{PictureController.GetPath()}/{guid}_PICTURE.png", pictureTexture, Helper.ImageType.PNG, TextureFormat.RGBA32, true);

        ToastController.Instance.Spawn("Picture taken");
    }

    private void OnEnable()
    {
        _controls.Enable();

        _rotation = transform.rotation.eulerAngles;
    }
    
    private void OnDisable() => _controls.Disable();
}
