using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

[System.Serializable]
public class LocalPlayerInfo
{
    private readonly InputDevice _device;
    public InputDevice Device => _device;

    private readonly InputUser _user;
    public InputUser User => _user;

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
