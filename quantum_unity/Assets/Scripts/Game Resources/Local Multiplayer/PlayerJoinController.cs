using UnityEngine.InputSystem;
using UnityEngine;
using Extensions.Types;
using System.Linq;
using FusionFighters.Profile;

public class PlayerJoinController : Extensions.Components.Miscellaneous.Controller<PlayerJoinController>
{
    private readonly Dictionary<InputDevice, LocalPlayerInfo> _allPlayers = new();
    public Dictionary<InputDevice, LocalPlayerInfo> AllPlayers => _allPlayers;

    private Controls _controls;

    private bool _isEnabled = true;
    public void Enable(bool isEnabled) => _isEnabled = isEnabled;

    private bool _executeEvents = true;
    public void SetExecuteEvents(bool isEnabled) => _executeEvents = isEnabled;

    public override void Initialize()
    {
        base.Initialize();

        _isEnabled = true;

        Subscribe();
        Application.quitting += Shutdown;
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _allPlayers.Clear();
        _controls = null;

        Serializer.Save(Profile.Instance, "Profile", $"{Application.persistentDataPath}");

        base.Shutdown();
    }

    public void Subscribe()
    {
        if (_controls is null)
        {
            _controls = new();

            _controls.Menu.Any.performed += TryPlayerJoin;
            _controls.Player.Leave.performed += TryPlayerLeave;

            _controls.Enable();
        }
    }

    private void TryPlayerJoin(InputAction.CallbackContext ctx)
    {
        if (!_isEnabled || GetPlayer(ctx.control.device) != null)
            return;

        LocalPlayerInfo player = AddPlayer(ctx.control.device);

        if (_executeEvents)
            foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                listener.InvokeOnPlayerJoin(player);
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (!_isEnabled || GetPlayer(ctx.control.device) == null)
            return;

        LocalPlayerInfo player = RemovePlayer(ctx.control.device);

        if (_executeEvents)
            foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            listener.InvokeOnPlayerLeave(player);
    }

    public LocalPlayerInfo AddPlayer(InputDevice device)
    {
        if (_allPlayers.ContainsKey(device))
            return _allPlayers[device];

        LocalPlayerInfo player = new(device);
        _allPlayers.Add(device, player);

        player.SetLocalIndex(player.User.index);
        Debug.Log($"Player {player.LocalIndex} has joined via {device?.displayName}");

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
}