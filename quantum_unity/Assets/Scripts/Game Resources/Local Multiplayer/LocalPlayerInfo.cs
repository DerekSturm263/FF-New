using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class LocalPlayerInfo
{
    private readonly InputDevice _device;
    public InputDevice Device => _device;

    private readonly InputUser _user;
    public InputUser User => _user;

    private UserProfile _profile;
    public UserProfile Profile => _profile;
    public void SetProfile(UserProfile profile) => _profile = profile;

    private int _localIndex;
    public int LocalIndex => _localIndex;
    public void SetLocalIndex(int index) => _localIndex = index;

    private int _globalIndex;
    public int GlobalIndex => _globalIndex;
    public void SetGlobalIndex(int index) => _globalIndex = index;

    public LocalPlayerInfo(InputDevice device)
    {
        if (device is not null)
        {
            _device = device;
            _user = InputUser.PerformPairingWithDevice(device);
        }
    }

    ~LocalPlayerInfo()
    {
        if (_user.id != InputUser.InvalidId && _device is not null)
            _user.UnpairDevice(_device);
    }
}
