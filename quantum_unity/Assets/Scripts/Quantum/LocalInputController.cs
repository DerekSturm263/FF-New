using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Extensions.Components.Miscellaneous;
using System.Collections;

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
            LookUp = player.Controls.Player.LookUp.IsPressed(),
            Burst = player.Controls.Player.Sub1.IsPressed() && player.Controls.Player.Sub2.IsPressed(),
            Block = player.Controls.Player.Block.IsPressed(),

            MainWeapon = player.Controls.Player.Primary.IsPressed(),
            AlternateWeapon = player.Controls.Player.Secondary.IsPressed(),
            SubWeapon = player.Controls.Player.Sub1.IsPressed() || player.Controls.Player.Sub2.IsPressed(),
            Ultimate = player.Controls.Player.Primary.IsPressed() && player.Controls.Player.Secondary.IsPressed(),

            Emote = player.Controls.Player.Emote.IsPressed(),
            Interact = player.Controls.Player.Interact.IsPressed(),

            Dodge = player.Controls.Player.Dodge.IsPressed(),
            Crouch = player.Controls.Player.Crouch.IsPressed(),
            Ready = player.Controls.Player.Ready.IsPressed(),
            Cancel = player.Controls.Player.Cancel.IsPressed()
        };

        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }

    private int _globalOffset;

    public void SpawnAllPlayers()
    {
        _globalOffset = 0;

        // May be causing a bug
        foreach (var player in PlayerJoinController.Instance.GetAllLocalPlayers(true))
        {
            SpawnPlayer(player);
        }
    }

    public void SpawnPlayer(LocalPlayerInfo player)
    {
        if (QuantumRunner.Default)
            SpawnPlayerImmediate(player);
        else
            FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStart.AddListener(_ => SpawnPlayerImmediate(player));
    }

    private void SpawnPlayerImmediate(LocalPlayerInfo player)
    {
        int offset = _globalOffset != -1 ? _globalOffset : 0;

        player.SetGlobalIndices
        (
            FighterIndex.GetNextGlobalIndex(QuantumRunner.Default.Game.Frames.Verified) + offset,
            FighterIndex.GetNextGlobalIndexNoBots(QuantumRunner.Default.Game.Frames.Verified) + offset
        );

        RuntimePlayer data = new()
        {
            CharacterPrototype = _player.CharacterPrototype,
            Name = player.Profile.Name,
            Index = player.Index,
            Build = player.Profile.value.LastBuild,
            IsRealBattle = FindFirstObjectByType<PlayerReadyEventListener>()
        };

        QuantumRunner.Default.Game.SendPlayerData(data.Index.Local, data);

        if (!gameObject.activeInHierarchy)
            player?.DisableMenus();

        Debug.Log($"Spawned player {player.Index}");

        if (_globalOffset == PlayerJoinController.Instance.GetPlayerCount(true) - 1)
            _globalOffset = -1;
        else if (_globalOffset != -1)
            ++_globalOffset;
    }

    public void SpawnAIDelayed(Bot bot, Sprite icon)
    {
        StartCoroutine(SpawnAIDelayedEnum(bot, icon));
    }

    private IEnumerator SpawnAIDelayedEnum(Bot bot, Sprite icon)
    {
        yield return new WaitForSeconds(0.5f);
        SpawnAI(bot, icon);
    }

    public void SpawnAI(Bot bot, Sprite icon)
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
                GlobalNoBots = -1,
                GlobalNoHumans = FighterIndex.GetNextGlobalIndexNoHumans(QuantumRunner.Default.Game.Frames.Verified),
                Type = FighterType.Bot,
            },
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
        if (!PlayerJoinController.Instance)
            return;

        foreach (var player in PlayerJoinController.Instance.GetAllLocalPlayers(false))
        {
            player.EnableMovement();
        }
    }

    private void OnDisable()
    {
        if (!PlayerJoinController.Instance)
            return;

        foreach (var player in PlayerJoinController.Instance.GetAllLocalPlayers(false))
        {
            player.DisableMovement();
        }
    }
}
