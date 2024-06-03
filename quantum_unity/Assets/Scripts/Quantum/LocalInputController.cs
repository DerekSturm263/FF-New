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

    private void Awake()
    {
        Initialize();
        
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback)
    {
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

    public void SpawnAllPlayers(QuantumGame game)
    {
        foreach (var player in LocalPlayerController.Instance.AllPlayers)
        {
            AddController(game, player.Value);
        }
    }

    public void SpawnPlayer(GamepadJoinCallbackContext ctx)
    {
        AddController(QuantumRunner.Default.Game, ctx.PlayerInfo);
    }

    public void AddController(QuantumGame game, LocalPlayerInfo player)
    {
        Controls controls = new();
        controls.Enable();

        int playerNum = QuantumRunner.Default.Game.GetLocalPlayers()[player.User.index];
        BindControls(controls, player, playerNum);

        game.SendPlayerData(playerNum, _player);

        _controls.Add(playerNum, controls);
    }

    public void BindControls(Controls controls, LocalPlayerInfo playerInfo, int playerNum)
    {
        if (playerInfo is null || playerInfo.User.id == InputUser.InvalidId)
            return;

        playerInfo.User.AssociateActionsWithUser(controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(playerInfo.Device, controls.controlSchemes);
        if (scheme.HasValue)
        {
            playerInfo.User.ActivateControlScheme(scheme.Value);
        }

        playerInfo.SetQuantumIndex(playerNum);
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
