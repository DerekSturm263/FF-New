using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
using FusionFighters.Profile;
using Quantum;
using System.Collections.Generic;

public class PlayerJoinController : Extensions.Components.Miscellaneous.Controller<PlayerJoinController>
{
    private List<LocalPlayerInfo> _allPlayers = new();
    public List<LocalPlayerInfo> LocalPlayers => _allPlayers;

    private Controls _controls;

    [SerializeField] private int _playerLimit;
    public void SetPlayerLimit(int playerLimit) => _instance._playerLimit = playerLimit;

    [SerializeField] private bool _isEnabled;
    public void Enable(bool isEnabled) => _instance._isEnabled = isEnabled;

    private bool _wasEnabled;

    public void DisableJoin()
    {
        _instance._wasEnabled = _instance._isEnabled;
        _instance._isEnabled = false;
    }

    public void TryEnableJoin()
    {
        _instance._isEnabled = _instance._wasEnabled;
    }

    private bool _executeEvents = true;
    public void SetExecuteEvents(bool isEnabled) => _executeEvents = isEnabled;

    [SerializeField] private ProfileAsset _default;

    private Profile _profile;
    public Profile Profile => _profile;

    [System.NonSerialized] private bool _isInitialized = false;

    public void SetProfileName(string name)
    {
        _profile.SetUsername(name);
        FindFirstObjectByType<DisplayProfile>().UpdateDisplay();
    }

    public override void Initialize()
    {
        base.Initialize();

        _playerLimit = 4;
        _isEnabled = true;

        Subscribe();

        if (!_isInitialized)
        {
            if (FusionFighters.Serializer.TryLoadAs($"{Application.persistentDataPath}/SaveData/Misc/Profile.json", $"{Application.persistentDataPath}/SaveData/Misc", out Profile profile))
                _profile = profile.DeepCopy();
            else
                _profile = _default.Profile.DeepCopy();

            _allPlayers.Clear();

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

        FusionFighters.Serializer.Save(_profile, "Profile", $"{Application.persistentDataPath}/SaveData/Misc");

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
        if (!_isEnabled || GetPlayer(ctx.control.device) != null || _allPlayers.Count == _playerLimit)
            return;

        LocalPlayerInfo player = AddPlayer(ctx.control.device);
        
        if (player is not null)
        {
            (UserProfileController.Instance as UserProfileController).SetPlayer(player);

            UserProfileController.Instance.Spawn(default);
            (UserProfileController.Instance as UserProfileController).DeferEvents(() =>
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
        if (_allPlayers.Any(item => item.Device == device))
            return _allPlayers.First(item => item.Device == device);

        int localIndex = GetNextLocalIndex();
        if (localIndex == -1)
            return null;

        FighterIndex index = new()
        {
            Local = localIndex,
            Device = HostClientEvents.DeviceIndex,
            Global = -1,
            Type = FighterType.Human
        };

        LocalPlayerInfo player = new(device, index);
        _allPlayers.Add(player);

        Debug.Log($"Player {player.Index} has joined via {device?.displayName}");

        return player;
    }

    public LocalPlayerInfo GetPlayer(InputDevice device)
    {
        var player = _allPlayers.FirstOrDefault(item => item.Device == device);
        if (player is not null)
            return player;

        return null;
    }

    public LocalPlayerInfo GetPlayer(int localIndex)
    {
        if (localIndex < 0 || localIndex >= _allPlayers.Count)
            return null;

        return _allPlayers.ElementAt(localIndex);
    }

    public LocalPlayerInfo RemovePlayer(InputDevice device) => RemovePlayer(GetPlayer(device));

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < _allPlayers.Count; ++i)
        {
            RemovePlayer(_allPlayers.First());
        }
    }

    public LocalPlayerInfo RemovePlayer(LocalPlayerInfo player)
    {
        if (player is not null)
            _allPlayers.Remove(player);

        Debug.Log($"Player has left the game");
        return player;
    }

    public int GetNextLocalIndex()
    {
        int[] indicesInUse = _allPlayers.Select(item => item.Index.Local).ToArray();
        
        for (int i = 0; i < 4; ++i)
        {
            if (!indicesInUse.Contains(i))
                return i;
        }

        return -1;
    }
}