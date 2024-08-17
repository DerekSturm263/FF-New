using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
using Quantum;
using System.Collections.Generic;
using System.Collections;
using Extensions.Components.Input;
using Photon.Realtime;

public class PlayerJoinController : Extensions.Components.Miscellaneous.Controller<PlayerJoinController>
{
    private List<LocalPlayerInfo> _localPlayers = new();

    public IEnumerable<LocalPlayerInfo> GetAllLocalPlayers(bool limitCount)
    {
        for (int i = 0; i <  _localPlayers.Count; ++i)
        {
            if (limitCount && i == _playerLimit)
                break;

            yield return _localPlayers[i];
        }
    }

    public int GetPlayerCount(bool limitCount) => GetAllLocalPlayers(limitCount).Count();

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

    [System.NonSerialized] private bool _isInitialized = false;

    private bool _executeEvents = true;
    public void SetExecuteEvents(bool isEnabled) => _executeEvents = isEnabled;

    public override void Initialize()
    {
        base.Initialize();

        _playerLimit = 4;
        _isEnabled = true;

        Subscribe();

        if (!_isInitialized)
        {
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
        if (!_isEnabled || TryGetPlayer(ctx.control.device, out LocalPlayerInfo _) || _localPlayers.Count == _playerLimit || InputEvent.IsInputting())
            return;

        if (TryAddPlayer(ctx.control.device, out LocalPlayerInfo player))
        {
            (UserProfileController.Instance as UserProfileController).SetPlayer(player);
            UserProfileController.Instance.Spawn(true);

            (UserProfileController.Instance as UserProfileController).DeferJoinEvents(() =>
            {
                if (_executeEvents)
                    foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Reverse())
                        listener.InvokeOnPlayerJoin(player);
            });
        }
    }

    private void TryPlayerLeave(InputAction.CallbackContext ctx)
    {
        if (!_isEnabled)
            return;
        
        if (TryGetPlayer(ctx.control.device, out LocalPlayerInfo player))
        {
            (UserProfileController.Instance as UserProfileController).SetPlayer(player);
            UserProfileController.Instance.Spawn(false);

            (UserProfileController.Instance as UserProfileController).DeferChangeEvents(() =>
            {
                CommandPlayerApplyProfile command = new()
                {
                    entity = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index),
                    name = player.Profile.Name
                };

                QuantumRunner.Default.Game.SendCommand(command);
            });

            (UserProfileController.Instance as UserProfileController).DeferLeaveEvents(() =>
            {
                if (_executeEvents)
                    foreach (var listener in FindObjectsByType<PlayerJoinEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Reverse())
                        listener.InvokeOnPlayerLeave(player);

                RemovePlayer(player);
            });
        }
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
            Global = localIndex,
            GlobalNoBots = localIndex,
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
        player = _localPlayers.FirstOrDefault(item => item.Index.Local == localIndex);
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