using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchController : Controller<MatchController>
{
    [SerializeField] private RuntimePlayer _ai;

    private Match _match;
    private Bot[] _opponents;

    public void LoadFromAsset(MatchAssetAsset match)
    {
        Match newMatch = Variate(match.Settings_MatchAsset.Match);

        _match = newMatch;
        _opponents = match.Settings_MatchAsset.Opponents;

        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += LoadScene;
    }

    private void LoadScene(Scene scene, LoadSceneMode loadMode)
    {
        RulesetController.Instance.Load(_match.Ruleset);
        StageController.Instance.Load(_match.Stage);

        QuantumRunnerLocalDebug runner = FindFirstObjectByType<QuantumRunnerLocalDebug>();
        foreach (Bot bot in _opponents)
        {
            runner.OnStart.AddListener(_ => SpawnAI(bot));
        }

        SceneManager.sceneLoaded -= LoadScene;
    }

    public void SpawnAI(Bot bot)
    {
        Bot newBot = Variate(bot);

        int localIndex = PlayerJoinController.Instance.GetNextLocalIndex();
        if (localIndex == -1)
            return;

        CommandSpawnAI commandSpawnAI = new()
        {
            prototype = _ai.CharacterPrototype,
            bot = newBot,
            index = new()
            {
                Local = localIndex,
                Device = HostClientEvents.DeviceIndex,
                Global = FighterIndex.GetNextGlobalIndex(QuantumRunner.Default.Game.Frames.Verified),
                Type = FighterType.Bot
            }
        };

        QuantumRunner.Default.Game.SendCommand(commandSpawnAI);
    }

    private Match Variate(Match match)
    {
        return new()
        {
            Ruleset = Variate(match.Ruleset),
            Stage = Variate(match.Stage)
        };
    }

    private Ruleset Variate(Ruleset ruleset)
    {
        return new()
        {
            Match = ruleset.Match,
            Players = ruleset.Players,
            Stage = ruleset.Stage,
            Items = ruleset.Items
        };
    }

    private Stage Variate(Stage stage)
    {
        return new()
        {
            Theme = stage.Theme,
            Objects = stage.Objects,
            Spawn = stage.Spawn
        };
    }

    private Bot Variate(Bot bot)
    {
        return new()
        {
            Build = bot.Build,
            Behavior = bot.Behavior
        };
    }

    public void GainCurrency()
    {
        if (QuantumRunner.Default.Game.Frames.Verified.TryGetSingleton(out Timer timer))
        {
            int diference = timer.OriginalTime - timer.Time;
            InventoryController.Instance.GainCurrency((ulong)(diference * 0.75f));
        }
    }
}
