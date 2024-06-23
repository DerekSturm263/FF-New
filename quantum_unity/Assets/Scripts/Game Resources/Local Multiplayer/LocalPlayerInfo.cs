using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class LocalPlayerInfo
{
    private readonly InputDevice _device;
    public InputDevice Device => _device;

    private readonly InputUser _user;
    public InputUser User => _user;

    private Controls _controls;
    public Controls Controls => _controls;

    private SerializableWrapper<UserProfile> _profile;
    public SerializableWrapper<UserProfile> Profile => _profile;
    public void SetProfile(SerializableWrapper<UserProfile> profile) => _profile = profile;

    private int _index;
    public int Index => _index;
    public void SetIndex(int index) => _index = index;

    public LocalPlayerInfo(InputDevice device)
    {
        if (device is not null)
        {
            _device = device;
            _user = InputUser.PerformPairingWithDevice(device);

            BindControls();
            Enable();
        }
    }

    public void BindControls()
    {
        _controls = new();
        _user.AssociateActionsWithUser(_controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(_device, _controls.controlSchemes);
        if (scheme.HasValue)
        {
            _user.ActivateControlScheme(scheme.Value);
        }
    }
    public void Enable() => _controls.Enable();
    public void Disable() => _controls.Disable();
}
