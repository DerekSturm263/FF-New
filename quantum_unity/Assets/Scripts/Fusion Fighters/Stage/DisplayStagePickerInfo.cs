using Quantum;
using Quantum.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayStagePickerInfo : UIBehaviour
{
    private TMPro.TMP_Text _text;

    protected override void Awake()
    {
        _text = GetComponent<TMPro.TMP_Text>();
    }

    protected override unsafe void OnEnable()
    {
        StagePicker stagePicker = QuantumRunner.Default.Game.Frames.Verified.FindAsset<StagePicker>(RulesetController.Instance.CurrentRuleset.value.Stage.StagePicker.Id);
        var (unsorted, sorted) = MatchSystem.GetTeams(QuantumRunner.Default.Game.Frames.Verified);

        var allowedPickers = sorted.Count() == 0 ?
            stagePicker.GetInitialPickers(QuantumRunner.Default.Game.Frames.Verified, unsorted) :
            stagePicker.GetAllowedPickers(QuantumRunner.Default.Game.Frames.Verified, sorted);

        if (allowedPickers.Count() == 0)
        {
            SetText(stagePicker.FallbackMessage);

            CommandMakeStageSelection command = new()
            {
                fighterIndex = FighterIndex.Invalid,
                stage = GetRandom()
            };

            QuantumRunner.Default.Game.SendCommand(command);
            return;
        }

        if (allowedPickers.Count() == QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers)
        {
            SetText(string.Format(stagePicker.FallbackMessage, 0, allowedPickers.Count()));
            return;
        }

        StringBuilder playerNumbers = new();

        foreach (var team in allowedPickers)
        {
            var players = team.Get(QuantumRunner.Default.Game.Frames.Verified);

            foreach (var player in players)
            {
                playerNumbers.Append($"Player {player.Global + 1}, ");
            }
        }

        playerNumbers.Remove(playerNumbers.Length - 2, 2);
        SetText(string.Format(stagePicker.SelectingMessage, playerNumbers.ToString(), 0));
    }

    public void SetText(string text)
    {
        _text.SetText(text);
    }

    private SerializableWrapper<Stage> GetRandom()
    {
        List<SerializableWrapper<Stage>> results = new();

        results.AddRange(Resources.LoadAll<StageAssetAsset>("DB/Assets/Stage/Stages").Where(item => item.IncludeInLists).Select(item => item.Stage));

        if (RulesetController.Instance.CurrentRuleset.value.Stage.AllowCustomStages)
            results.AddRange(FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<Stage>>(StageController.GetPath()));

        return results[Random.Range(0, results.Count)];
    }

}
