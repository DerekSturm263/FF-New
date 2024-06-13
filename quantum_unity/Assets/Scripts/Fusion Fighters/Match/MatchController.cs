using Extensions.Components.Miscellaneous;
using Quantum;
using Quantum.Types;
using UnityEngine;

public class MatchController : Controller<MatchController>
{
    [SerializeField] private RuntimePlayer _ai;

    private Match _match;
    public Match Match => _match;

    public void LoadFromAsset(MatchAssetAsset match)
    {
        Match newMatch = Variate(match.Settings_MatchAsset.Match);
        _match = newMatch;

        RulesetController.Instance.Load(_match.Ruleset);
        StageController.Instance.Load(_match.Stage);

        QuantumRunnerLocalDebug runner = FindFirstObjectByType<QuantumRunnerLocalDebug>();
        for (int i = 0; i < 2; ++i)
        {
            Bot bot = ArrayHelper.Get(match.Settings_MatchAsset.Opponents, i);

            if (!bot.Equals(default))
                runner.OnStartDeferred.AddListener(_ => SpawnAI(bot));
        }
    }

    public void SpawnAI(Bot bot)
    {
        Bot newBot = Variate(bot);
        
        CommandSpawnAI commandSpawnAI = new()
        {
            prototype = _ai.CharacterPrototype,
            bot = newBot
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
            InventoryController.Instance.GainCurrency(diference / 2);
        }
    }
}
