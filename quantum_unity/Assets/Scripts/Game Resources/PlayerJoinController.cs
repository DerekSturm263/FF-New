using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
using FusionFighters.Profile;
using Quantum;
using System.Collections.Generic;
using System.Collections;

public class PlayerJoinController : Extensions.Components.Miscellaneous.Controller<PlayerJoinController>
{
    private List<LocalPlayerInfo> _localPlayers = new();
    public List<LocalPlayerInfo> LocalPlayers => _localPlayers;

    private Controls _controls;

    [SerializeField] private int _playerLimit;
    public int PlayerLimit => _playerLimit;
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

            _localPlayers.Clear();

            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        _localPlayers.Clear();
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
        if (!_isEnabled || TryGetPlayer(ctx.control.device, out LocalPlayerInfo _) || _localPlayers.Count == _playerLimit)
            return;

        if (TryAddPlayer(ctx.control.device, out LocalPlayerInfo player))
        {
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
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (!_isEnabled || !TryGetPlayer(ctx.control.device, out LocalPlayerInfo _))
            return;

        LocalPlayerInfo player = RemovePlayer(ctx.control.device);
        FindFirstObjectByType<DisplayUsers>()?.UpdateDisplay();

        if (_executeEvents)
            foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                listener.InvokeOnPlayerLeave(player);
    }

    public bool TryAddPlayer(InputDevice device, out LocalPlayerInfo player)
    {
        if (_localPlayers.Any(item => item.Device == device))
        {
            player = _localPlayers.First(item => item.Device == device);
            return false;
        }

        int localIndex = GetNextLocalIndex();
        if (localIndex == -1)
        {
            player = null;
            return false;
        }

        FighterIndex index = new()
        {
            Local = localIndex,
            Device = HostClientEvents.DeviceIndex,
            GlobalNoHumans = -1,
            Type = FighterType.Human
        };

        player = new(device, index);
        _localPlayers.Add(player);

        Debug.Log($"Player {player.Index} has joined via {device?.displayName}");

        return true;
    }

    public bool TryGetPlayer(InputDevice device, out LocalPlayerInfo player)
    {
        player = _localPlayers.FirstOrDefault(item => item.Device == device);
        if (player is not null)
            return true;

        return false;
    }

    public bool TryGetPlayer(FighterIndex index, out LocalPlayerInfo player)
    {
        player = _localPlayers.FirstOrDefault(item => item.Index.Equals(index));
        if (player is not null)
            return true;

        return false;
    }

    public bool TryGetPlayer(int localIndex, out LocalPlayerInfo player)
    {
        player = _localPlayers.ElementAtOrDefault(localIndex);
        if (player is not null)
            return true;

        return false;
    }

    public LocalPlayerInfo RemovePlayer(InputDevice device)
    {
        if (TryGetPlayer(device, out LocalPlayerInfo player))
            return player;

        return null;
    }

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < _localPlayers.Count; ++i)
        {
            RemovePlayer(_localPlayers.First());
        }
    }

    public LocalPlayerInfo RemovePlayer(LocalPlayerInfo player)
    {
        if (player is not null)
            _localPlayers.Remove(player);

        Debug.Log($"Player has left the game");
        return player;
    }

    public int GetNextLocalIndex()
    {
        int[] indicesInUse = _localPlayers.Select(item => item.Index.Local).ToArray();
        
        for (int i = 0; i < 4; ++i)
        {
            if (!indicesInUse.Contains(i))
                return i;
        }

        return -1;
    }

    public void Rumble(LocalPlayerInfo player, float frequency, float time) => CoroutineRunner.Instance.StartCoroutine(RumbleCoroutine(player, frequency, time));

    private IEnumerator RumbleCoroutine(LocalPlayerInfo player, float frequency, float time)
    {
        if (player.Device is Gamepad gamepad)
        {
            gamepad.SetMotorSpeeds(frequency, frequency);

            gamepad.ResumeHaptics();
            yield return new WaitForSeconds(time);
            gamepad.PauseHaptics();
        }
    }
}