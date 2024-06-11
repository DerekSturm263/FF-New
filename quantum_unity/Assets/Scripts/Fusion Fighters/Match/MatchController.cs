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
        _match = match.Settings_MatchAsset.Match;

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
        CommandSpawnAI commandSpawnAI = new()
        {
            prototype = _ai.CharacterPrototype,
            bot = bot
        };

        QuantumRunner.Default.Game.SendCommand(commandSpawnAI);
    }
}
