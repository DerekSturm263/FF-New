using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;
using Extensions.Types;
using System.Linq;
using UnityEngine.InputSystem.UI;

public class LocalPlayerController : Extensions.Components.Miscellaneous.Controller<LocalPlayerController>
{
    private Dictionary<InputDevice, LocalPlayerInfo> _allPlayers;
    public Dictionary<InputDevice, LocalPlayerInfo> AllPlayers => _allPlayers;

    [SerializeField] private UnityEvent<GamepadJoinCallbackContext> _onPlayerJoin;
    [SerializeField] private UnityEvent<GamepadJoinCallbackContext> _onPlayerLeave;

    [SerializeField] private RectTransform _canvas;
    [SerializeField] private GameObject _css;

    private Controls _controls;

    private void Awake()
    {
        _allPlayers ??= new();
        _controls = new();

        _controls.Player.Join.performed += TryPlayerJoin;
        _controls.Player.Leave.performed += TryPlayerLeave;
    }

    private void TryPlayerJoin(InputAction.CallbackContext ctx)
    {
        if (GetPlayer(ctx.control.device) != null)
            return;

        _onPlayerJoin.Invoke(new(ctx, AddPlayer(ctx.control.device)));
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (GetPlayer(ctx.control.device) == null)
            return;

        _onPlayerLeave.Invoke(new(ctx, RemovePlayer(ctx.control.device)));
    }

    public LocalPlayerInfo AddPlayer(InputDevice device)
    {
        if (_allPlayers.ContainsKey(device))
            return _allPlayers[device];

        LocalPlayerInfo player = new(device);
        _allPlayers.Add(device, player);

        GameObject eventSystem = Instantiate(_css, _canvas.transform);
        eventSystem.GetComponentInChildren<UserUILink>().BindControls(player);

        Debug.Log($"Player has joined via {device?.displayName}");

        return player;
    }

    public LocalPlayerInfo GetPlayer(InputDevice device)
    {
        if (_allPlayers.TryGetValue(device, out var player))
            return player;

        return null;
    }

    public LocalPlayerInfo RemovePlayer(InputDevice device) => RemovePlayer(GetPlayer(device));

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < _allPlayers.Count; ++i)
        {
            RemovePlayer(_allPlayers.First().Value);
        }
    }

    private LocalPlayerInfo RemovePlayer(LocalPlayerInfo player)
    {
        if (player is not null)
            _allPlayers.Remove(player.Device);

        Debug.Log($"Player has left the game");
        return player;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}