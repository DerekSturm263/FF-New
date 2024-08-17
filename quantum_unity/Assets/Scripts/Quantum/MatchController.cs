using Extensions.Components.Miscellaneous;
using Quantum;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchController : Controller<MatchController>
{
    private Match _match;
    private (Bot, Sprite)[] _opponents;

    public void LoadFromAsset(MatchAssetAsset match)
    {
        Match newMatch = Variate(match.Settings_MatchAsset.Match);

        _match = newMatch;
        _opponents = match.Settings_MatchAsset.Opponents.Zip(match.BotSprites, (item1, item2) => (item1, item2)).ToArray();

        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += LoadScene;
    }

    private void LoadScene(Scene scene, LoadSceneMode loadMode)
    {
        RulesetController.Instance.Load(_match.Ruleset);
        StageController.Instance.Load(_match.Stage);

        QuantumRunnerLocalDebug runner = FindFirstObjectByType<QuantumRunnerLocalDebug>();
        foreach (var bot in _opponents)
        {
            runner.OnStart.AddListener(_ => LocalInputController.Instance.SpawnAIDelayed(Variate(bot.Item1), bot.Item2));
        }

        SceneManager.sceneLoaded -= LoadScene;
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
            Behavior = bot.Behavior,
            Name = bot.Name
        };
    }

    public void GainCurrency()
    {
        if (QuantumRunner.Default.Game.Frames.Verified.TryGetSingleton(out Timer timer))
        {
            int diference = timer.OriginalTime - timer.Time;
            InventoryController.Instance.GainCurrency((uint)(diference * 0.75f));
        }
    }
}
