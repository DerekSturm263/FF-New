using Quantum;
using UnityEngine.InputSystem;

public class GamepadJoinCallbackContext
{
    private readonly InputAction.CallbackContext _inputCallbackContext;
    public InputAction.CallbackContext CallbackContext => _inputCallbackContext;

    private readonly LocalPlayerInfo _playerInfo;
    public LocalPlayerInfo PlayerInfo => _playerInfo;

    public GamepadJoinCallbackContext(InputAction.CallbackContext inputCallbackContext, LocalPlayerInfo playerInfo)
    {
        _inputCallbackContext = inputCallbackContext;
        _playerInfo = playerInfo;
    }
}