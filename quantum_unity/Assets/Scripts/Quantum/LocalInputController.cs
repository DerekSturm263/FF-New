using Photon.Deterministic;
using Quantum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
using Extensions.Components.Miscellaneous;

public class LocalInputController : Controller<LocalInputController>
{
    [SerializeField] private RuntimePlayer _player;

    private readonly Dictionary<int, Controls> _controls = new();

    private static bool _canInput;
    public void SetCanInput(bool canInput) => _canInput = canInput;

    private void Awake()
    {
        Initialize();
        
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback)
    {
        if (!_canInput)
            return;

        if (!_controls.ContainsKey(callback.Player))
        {
            callback.SetInput(new(), DeterministicInputFlags.Repeatable);
            return;
        }

        Controls controls = _controls[callback.Player];

        Quantum.Input input = new()
        {
            Movement = controls.Player.Move.ReadValue<Vector2>().ToFPVector2(),

            Jump = controls.Player.Jump.IsPressed(),
            FastFall = controls.Player.FastFall.IsPressed(),
            Crouch = controls.Player.Crouch.IsPressed(),
            Block = controls.Player.Block.IsPressed(),

            MainWeapon = controls.Player.MainWeapon.IsPressed(),
            AlternateWeapon = controls.Player.AlternateWeapon.IsPressed(),
            SubWeapon1 = controls.Player.Subweapon1.IsPressed(),
            SubWeapon2 = controls.Player.Subweapon2.IsPressed(),

            Emote = controls.Player.Emote.IsPressed(),
            Interact = controls.Player.Interact.IsPressed(),

            Ready = controls.Player.Ready.IsPressed(),
            Cancel = controls.Player.Cancel.IsPressed()
        };

        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }

    public void SpawnAllPlayers()
    {
        foreach (var player in PlayerJoinController.Instance.AllPlayers)
        {
            SpawnPlayer(player.Value);
        }
    }

    public void SpawnPlayer(LocalPlayerInfo player)
    {
        QuantumRunner.Default.Game.SendPlayerData(player.LocalIndex, _player);

        Debug.Log($"Spawned player {player.LocalIndex}");
    }

    public void ApplyProfile(LocalPlayerInfo player)
    {
        CommandPlayerApplyProfile command = new()
        {
            entity = BuildController.Instance.GetPlayerLocalIndex(player.LocalIndex),
            name = player.Profile.Name
        };

        QuantumRunner.Default.Game.SendCommand(command);
    }

    public void DespawnPlayer(LocalPlayerInfo player)
    {
        CommandDespawnPlayer commandDespawnPlayer = new()
        {
            entity = BuildController.Instance.GetPlayerLocalIndex(player.LocalIndex)
        };

        UnbindPlayerControls(player);
        QuantumRunner.Default.Game.SendCommand(commandDespawnPlayer);
    }

    public void BindPlayerControls(LocalPlayerInfo player)
    {
        Controls controls = new();

        if (gameObject.activeInHierarchy)
            controls.Enable();

        BindControls(controls, player, player.LocalIndex);
        _controls.TryAdd(player.LocalIndex, controls);

        Debug.Log($"Created movement controls for local player {player.LocalIndex}");
    }

    public void UnbindPlayerControls(LocalPlayerInfo player)
    {
        _controls.Remove(player.LocalIndex);
    }

    private void BindControls(Controls controls, LocalPlayerInfo playerInfo, int playerNum)
    {
        if (playerInfo is null || playerInfo.User.id == InputUser.InvalidId)
            return;

        playerInfo.User.AssociateActionsWithUser(controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(playerInfo.Device, controls.controlSchemes);
        if (scheme.HasValue)
        {
            playerInfo.User.ActivateControlScheme(scheme.Value);
        }

        //playerInfo.SetGlobalIndex(playerNum);
    }

    private void OnEnable()
    {
        foreach (Controls controls in _controls.Values)
        {
            controls.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (Controls controls in _controls.Values)
        {
            controls.Disable();
        }
    }
}
