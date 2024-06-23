using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Extensions.Components.Miscellaneous;

public class LocalInputController : Controller<LocalInputController>
{
    [SerializeField] private RuntimePlayer _player;

    private static bool _canInput;
    public void SetCanInput(bool canInput) => _canInput = canInput;

    private void Awake()
    {
        Initialize();
        _canInput = false;
        
        QuantumCallback.Subscribe<CallbackPollInput>(this, PollInput);
    }

    public void PollInput(CallbackPollInput callback)
    {
        if (!_canInput)
            return;

        LocalPlayerInfo player = PlayerJoinController.Instance.GetPlayer(callback.Player);
        if (player is null)
        {
            callback.SetInput(new(), DeterministicInputFlags.Repeatable);
            return;
        }

        Quantum.Input input = new()
        {
            Movement = player.Controls.Player.Move.ReadValue<Vector2>().ToFPVector2(),

            Jump = player.Controls.Player.Jump.IsPressed(),
            FastFall = player.Controls.Player.FastFall.IsPressed(),
            Crouch = player.Controls.Player.Crouch.IsPressed(),
            Block = player.Controls.Player.Block.IsPressed(),

            MainWeapon = player.Controls.Player.MainWeapon.IsPressed(),
            AlternateWeapon = player.Controls.Player.AlternateWeapon.IsPressed(),
            SubWeapon1 = player.Controls.Player.Subweapon1.IsPressed(),
            SubWeapon2 = player.Controls.Player.Subweapon2.IsPressed(),

            Emote = player.Controls.Player.Emote.IsPressed(),
            Interact = player.Controls.Player.Interact.IsPressed(),

            Ready = player.Controls.Player.Ready.IsPressed(),
            Cancel = player.Controls.Player.Cancel.IsPressed()
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
        QuantumRunner.Default.Game.SendPlayerData(player.Index, new() { CharacterPrototype = _player.CharacterPrototype, DeviceIndex = HostClientEvents.DeviceIndex, Name = player.Profile.Name });

        if (!gameObject.activeInHierarchy)
            player?.Controls?.Menu.Disable();

        Debug.Log($"Spawned player {player.Index}");
    }

    public void ApplyProfile(LocalPlayerInfo player)
    {
        // TODO: FIX THE BELOW. IT WILL NOT WORK WHEN BOTS ARE IN.
        CommandPlayerApplyProfile command = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index, HostClientEvents.DeviceIndex),
            name = player.Profile.Name
        };

        QuantumRunner.Default.Game.SendCommand(command);
    }

    public void DespawnPlayer(LocalPlayerInfo player)
    {
        // TODO: FIX THE BELOW. IT WILL NOT WORK WHEN BOTS ARE IN.
        CommandDespawnPlayer commandDespawnPlayer = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index, HostClientEvents.DeviceIndex)
        };

        QuantumRunner.Default.Game.SendCommand(commandDespawnPlayer);
    }

    private void OnEnable()
    {
        foreach (var player in PlayerJoinController.Instance.AllPlayers.Values)
        {
            player.Controls?.Player.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var player in PlayerJoinController.Instance.AllPlayers.Values)
        {
            player.Controls?.Player.Disable();
        }
    }
}
