using Quantum;
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
    public ref SerializableWrapper<UserProfile> Profile => ref _profile;
    public void SetProfile(SerializableWrapper<UserProfile> profile) => _profile = profile;

    private FighterIndex _index;
    public FighterIndex Index => _index;
    public void SetGlobalIndices(int global, int globalNoBots)
    {
        _index.Global = global;
        _index.GlobalNoBots = globalNoBots;
    }
    public void SetTeamIndex(int team)
    {
        _index.Team = team;
    }

    public LocalPlayerInfo(InputDevice device, FighterIndex index)
    {
        if (device is not null)
        {
            _device = device;
            _user = InputUser.PerformPairingWithDevice(device);

            BindControls();
            EnableAll();
        }

        _index = index;
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

    public void EnableAll() => _controls?.Enable();
    public void DisableAll() => _controls?.Disable();

    public void EnableMenus() => _controls?.Menu.Enable();
    public void DisableMenus() => _controls?.Menu.Disable();

    public void EnableMovement() => _controls?.Player.Enable();
    public void DisableMovement() => _controls?.Player.Disable();
}
