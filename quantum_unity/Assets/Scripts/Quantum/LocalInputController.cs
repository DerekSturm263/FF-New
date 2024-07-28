using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Extensions.Components.Miscellaneous;
using UnityEngine.SceneManagement;

public class LocalInputController : Controller<LocalInputController>
{
    [SerializeField] private RuntimePlayer _player;
    [SerializeField] private RuntimePlayer _bot;

    [SerializeField] private bool _canInput;
    public void SetCanInput(bool canInput) => _instance._canInput = canInput;

    private bool _couldInput;

    public void DisableInput()
    {
        _instance._couldInput = _instance._canInput;
        _instance._canInput = false;
    }

    public void TryEnableInput()
    {
        _instance._canInput = _instance._couldInput;
    }

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

        if (!PlayerJoinController.Instance.TryGetPlayer(callback.Player, out LocalPlayerInfo player))
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
        foreach (var player in PlayerJoinController.Instance.LocalPlayers)
        {
            SpawnPlayer(player);
        }
    }

    public void SpawnPlayer(LocalPlayerInfo player)
    {
        player.SetGlobalIndices(FighterIndex.GetNextGlobalIndex(QuantumRunner.Default.Game.Frames.Verified), FighterIndex.GetNextGlobalIndexNoBots(QuantumRunner.Default.Game.Frames.Verified));

        RuntimePlayer data = new()
        {
            CharacterPrototype = _player.CharacterPrototype,
            Name = player.Profile.Name,
            Index = player.Index,
            IsRealBattle = FindFirstObjectByType<PlayerReadyEventListener>()
        };

        QuantumRunner.Default.Game.SendPlayerData(data.Index.Local, data);

        if (!gameObject.activeInHierarchy)
            player?.Controls?.Menu.Disable();

        Debug.Log($"Spawned player {player.Index}");
    }

    public void SpawnAI(Bot bot)
    {
        int localIndex = PlayerJoinController.Instance.GetNextLocalIndex();
        if (localIndex == -1)
            return;

        CommandSpawnAI commandSpawnAI = new()
        {
            prototype = _bot.CharacterPrototype,
            bot = bot,
            name = bot.Name,
            index = new()
            {
                Local = localIndex,
                Device = HostClientEvents.DeviceIndex,
                Global = FighterIndex.GetNextGlobalIndex(QuantumRunner.Default.Game.Frames.Verified),
                Type = FighterType.Bot
            }
        };

        QuantumRunner.Default.Game.SendCommand(commandSpawnAI);
        Debug.Log($"Spawned bot {commandSpawnAI.index}");
    }

    public void ApplyProfile(LocalPlayerInfo player)
    {
        CommandPlayerApplyProfile command = new()
        {
            entity = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index),
            name = player.Profile.Name
        };

        QuantumRunner.Default.Game.SendCommand(command);
    }

    public void DespawnPlayer(LocalPlayerInfo player)
    {
        CommandDespawnPlayer commandDespawnPlayer = new()
        {
            entity = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index)
        };

        QuantumRunner.Default.Game.SendCommand(commandDespawnPlayer);
    }

    private void OnEnable()
    {
        foreach (var player in PlayerJoinController.Instance.LocalPlayers)
        {
            player.Controls?.Player.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var player in PlayerJoinController.Instance.LocalPlayers)
        {
            player.Controls?.Player.Disable();
        }
    }
}
