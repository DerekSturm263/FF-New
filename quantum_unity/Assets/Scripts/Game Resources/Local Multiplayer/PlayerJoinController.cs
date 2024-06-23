using UnityEngine.InputSystem;
using UnityEngine;
using Extensions.Types;
using System.Linq;
using FusionFighters.Profile;

public class PlayerJoinController : Extensions.Components.Miscellaneous.Controller<PlayerJoinController>
{
    [System.NonSerialized] private Dictionary<InputDevice, LocalPlayerInfo> _allPlayers;
    public Dictionary<InputDevice, LocalPlayerInfo> AllPlayers => _allPlayers;

    private Controls _controls;

    private int _playerLimit;
    public void SetPlayerLimit(int playerLimit) => _playerLimit = playerLimit;

    private bool _isEnabled = true;
    public void Enable(bool isEnabled) => _isEnabled = isEnabled;

    private bool _executeEvents = true;
    public void SetExecuteEvents(bool isEnabled) => _executeEvents = isEnabled;

    [System.NonSerialized] private bool _isInitialized = false;

    public override void Initialize()
    {
        base.Initialize();

        _isEnabled = true;
        _playerLimit = 4;

        Subscribe();

        if (!_isInitialized)
        {
            _allPlayers = new();

            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        _allPlayers.Clear();
        _controls = null;

        Serializer.Save(Profile.Instance, "Profile", $"{Application.persistentDataPath}/SaveData/Misc");

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
        if (!_isEnabled || UserProfileController.Instance.IsSpawningPlayer || GetPlayer(ctx.control.device) != null)
            return;

        if (_allPlayers.Count == _playerLimit)
            return;

        LocalPlayerInfo player = AddPlayer(ctx.control.device);
        
        if (player is not null)
        {
            UserProfileController.Instance.SetPlayer(player);

            UserProfileController.Instance.Spawn();
            UserProfileController.Instance.DeferEvents(() =>
            {
                if (_executeEvents)
                    foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Reverse())
                        listener.InvokeOnPlayerJoin(player);
            });
        }
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (!_isEnabled || GetPlayer(ctx.control.device) == null)
            return;

        LocalPlayerInfo player = RemovePlayer(ctx.control.device);
        FindFirstObjectByType<DisplayUsers>()?.UpdateDisplay();

        if (_executeEvents)
            foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                listener.InvokeOnPlayerLeave(player);
    }

    public LocalPlayerInfo AddPlayer(InputDevice device)
    {
        if (_allPlayers.ContainsKey(device))
            return _allPlayers[device];

        int index = NextIndex();
        if (index == -1)
            return null;

        LocalPlayerInfo player = new(device);
        _allPlayers.Add(device, player);

        player.SetIndex(index);
        Debug.Log($"Player {player.Index} has joined via {device?.displayName}");

        return player;
    }

    public LocalPlayerInfo GetPlayer(InputDevice device)
    {
        if (_allPlayers.TryGetValue(device, out var player))
            return player;

        return null;
    }

    public LocalPlayerInfo GetPlayer(int localIndex)
    {
        if (localIndex < 0 || localIndex >= _allPlayers.Count)
            return null;

        return _allPlayers.ElementAt(localIndex).Value;
    }

    public LocalPlayerInfo RemovePlayer(InputDevice device) => RemovePlayer(GetPlayer(device));

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < _allPlayers.Count; ++i)
        {
            RemovePlayer(_allPlayers.First().Value);
        }
    }

    public LocalPlayerInfo RemovePlayer(LocalPlayerInfo player)
    {
        if (player is not null)
            _allPlayers.Remove(player.Device);

        Debug.Log($"Player has left the game");
        return player;
    }

    public int NextIndex()
    {
        int[] indicesInUse = _allPlayers.Values.Select(item => item.Index).ToArray();
        
        for (int i = 0; i < 4; ++i)
        {
            if (!indicesInUse.Contains(i))
                return i;
        }

        return -1;
    }
}