using Extensions.Miscellaneous;
using Quantum;
using System.Collections.Generic;
using System.Linq;
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
        }
        else if (stagePicker.GetPlayerCountToDecide(QuantumRunner.Default.Game.Frames.Verified, QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers) == QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers)
        {
            string message = stagePicker.FallbackMessage;

            if (QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers > 1)
                message = message.Replace("is", "are");

            string playerNames = string.Format(message, 0, QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers);

            SetText(playerNames);
        }
        else if (allowedPickers.Count() == QuantumRunner.Default.Game.Frames.Verified.Global->TotalPlayers)
        {
            string message = stagePicker.FallbackMessage;

            if (allowedPickers.Count() > 1)
                message = message.Replace("is", "are");
            
            string playerNames = string.Format(message, 0, allowedPickers.Count());

            SetText(playerNames);
        }
        else
        {
            string message = stagePicker.SelectingMessage;
            
            string playerNumbers = Helper.PrintNames(allowedPickers, item => Helper.PrintNames(item.Get(QuantumRunner.Default.Game.Frames.Verified), item2 => item2.Name, and: "and"), and: "and");

            if (allowedPickers.Sum(item => item.Get(QuantumRunner.Default.Game.Frames.Verified).Count()) > 1)
                message = message.Replace("is", "are");
            
            string playerNames = string.Format(message, playerNumbers.ToString(), 0);

            SetText(playerNames);
        }
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
