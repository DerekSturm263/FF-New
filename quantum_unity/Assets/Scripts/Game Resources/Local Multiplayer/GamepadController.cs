using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;

public class GamepadController : Extensions.Components.Miscellaneous.Controller<GamepadController>
{
    private Controls _controls;

    [SerializeField] private UnityEvent<GamepadJoinCallbackContext> _onPlayerJoin;
    [SerializeField] private UnityEvent<GamepadJoinCallbackContext> _onPlayerLeave;

    [SerializeField] private bool _resetAllPlayers;

    private void Awake()
    {
        if (_resetAllPlayers)
        {
            LocalPlayerInfo.RemoveAllPlayers();
        }

        _controls = new();

        _controls.Player.Join.performed += TryPlayerJoin;
        _controls.Player.Leave.performed += TryPlayerLeave;
    }

    private void Start()
    {
        foreach (LocalPlayerInfo player in LocalPlayerInfo.AllPlayers.Values)
        {
            _onPlayerJoin.Invoke(new(default, player));
        }
    }

    private void TryPlayerJoin(InputAction.CallbackContext ctx)
    {
        if (LocalPlayerInfo.GetPlayer(ctx.control.device) != null)
            return;

        _onPlayerJoin.Invoke(new(ctx, LocalPlayerInfo.AddPlayer(ctx.control.device)));
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (LocalPlayerInfo.GetPlayer(ctx.control.device) == null)
            return;

        _onPlayerLeave.Invoke(new(ctx, LocalPlayerInfo.RemovePlayer(ctx.control.device)));
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}