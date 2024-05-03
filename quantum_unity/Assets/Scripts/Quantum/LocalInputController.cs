using Photon.Deterministic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;

public class LocalInputController : MonoBehaviour
{
    private Controls _controls;

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Awake()
    {
        _controls = new();
    }

    private void Start()
    {
        QuantumCallback.Subscribe(this, (Quantum.CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(Quantum.CallbackPollInput callback)
    {
        Quantum.Input input = new()
        {
            Movement = _controls.Player.Move.ReadValue<Vector2>().ToFPVector2(),

            Jump = _controls.Player.Jump.IsPressed(),
            FastFall = _controls.Player.FastFall.IsPressed(),
            Crouch = _controls.Player.Crouch.IsPressed(),
            Block1 = _controls.Player.Block1.IsPressed(),
            Block2 = _controls.Player.Block2.IsPressed(),

            MainWeapon = _controls.Player.MainWeapon.IsPressed(),
            AlternateWeapon = _controls.Player.AlternateWeapon.IsPressed(),
            SubWeapon = _controls.Player.Subweapon.IsPressed(),

            Emote = _controls.Player.Emote.IsPressed(),
            Interact = _controls.Player.Interact.IsPressed(),

            Ready = _controls.Player.Ready.IsPressed(),
            Cancel = _controls.Player.Cancel.IsPressed()
        };

        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }

    public void BindControls(LocalPlayerInfo playerInfo)
    {
        if (playerInfo is not null || playerInfo.User.id == InputUser.InvalidId)
            return;

        playerInfo.User.AssociateActionsWithUser(_controls);

        InputControlScheme? scheme = InputControlScheme.FindControlSchemeForDevice(playerInfo.Device, _controls.controlSchemes);
        if (scheme.HasValue)
        {
            playerInfo.User.ActivateControlScheme(scheme.Value);
        }
    }
}
