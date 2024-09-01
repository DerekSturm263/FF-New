using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    [SerializeField] private RulesetAssetAsset _default;
    [SerializeField] private RulesetAssetAsset _none;

    [SerializeField] private Popup _savePopup;

    private bool _isDirty;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Rulesets";

    private SerializableWrapper<Ruleset> _currentRuleset;
    public SerializableWrapper<Ruleset> CurrentRuleset => _currentRuleset;
    public void Select(SerializableWrapper<Ruleset> ruleset)
    {
        _currentRuleset = ruleset;
        _isDirty = false;
    }

    public override void Initialize()
    {
        base.Initialize();

        _currentRuleset = _none.Ruleset;
    }

    public void New()
    {
        Ruleset ruleset = _default.Ruleset.value;
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        _currentRuleset = new(ruleset, GetPath(), "Untitled", "", AssetGuid.NewGuid(), filterTags, groupTags);
        _isDirty = true;
    }

    public void Save(SerializableWrapper<Ruleset> ruleset)
    {
        ruleset.Save();
    }

    public void SaveCurrent()
    {
        _currentRuleset.Save();
        _isDirty = false;

        ToastController.Instance.Spawn("Ruleset saved");
    }

    public void CloseOrSaveConfirm(InvokableGameObject invokable)
    {
        if (_isDirty)
        {
            (PopupController.Instance as PopupController).InsertEvent(invokable);
            PopupController.Instance.Spawn(_savePopup);
        }
        else
        {
            invokable.Invoke();
        }
    }

    public void SetName(string name)
    {
        _currentRuleset.SetName(name);
        _isDirty = true;
    }

    public void SetDescription(string description)
    {
        _currentRuleset.SetDescription(description);
        _isDirty = true;
    }

    public void Delete()
    {
        _currentRuleset.Delete();

        if (RulesetPopulator.Instance && RulesetPopulator.Instance.TryGetButtonFromItem(_currentRuleset, out GameObject button))
        {
            DestroyImmediate(button);
            RulesetPopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }


        ToastController.Instance.Spawn("Ruleset deleted");
    }

    public void SetWinCondition(WinConditionAsset winCondition)
    {
        _currentRuleset.value.Match.WinCondition = new() { Id = winCondition.AssetObject.Guid };
        _isDirty = true;
    }

    public void SetTieResolver(TieResolverAsset tieResolver)
    {
        _currentRuleset.value.Match.TieResolver = new() { Id = tieResolver.AssetObject.Guid };
        _isDirty = true;
    }

    public void SetTime(string time)
    {
        _currentRuleset.value.Match.Time = Convert.ToInt32(time);
        _isDirty = true;
    }

    public void SetShowTimer(bool showTimer)
    {
        _currentRuleset.value.Match.ShowTimer = showTimer;
        _isDirty = true;
    }

    public void SetMatchCount(string matchCount)
    {
        _currentRuleset.value.Match.MatchCount = Convert.ToInt32(matchCount);
        _isDirty = true;
    }

    public void SetEndMatchesWhenWinnerClear(bool endMatchesWhenWinnerClear)
    {
        _currentRuleset.value.Match.EndMatchesWhenWinnerClear = endMatchesWhenWinnerClear;
        _isDirty = true;
    }

    public void ShowScores(bool showScores)
    {
        _currentRuleset.value.Match.ShowScores = showScores;
        _isDirty = true;
    }

    public void ShowCurrentWinner(bool showCurrentWinner)
    {
        _currentRuleset.value.Match.ShowCurrentWinner = showCurrentWinner;
        _isDirty = true;
    }

    public void SetStockCount(string stockCount)
    {
        _currentRuleset.value.Players.StockCount = Convert.ToInt32(stockCount);
        _isDirty = true;
    }

    public void SetMaxHealth(string maxHealth)
    {
        _currentRuleset.value.Players.MaxHealth = Convert.ToInt32(maxHealth);
        _isDirty = true;
    }

    public void SetMaxEnergy(string maxEnergy)
    {
        _currentRuleset.value.Players.MaxEnergy = Convert.ToInt32(maxEnergy);
        _isDirty = true;
    }

    public void SetEnergyChargeRate(float energyChargeRate)
    {
        _currentRuleset.value.Players.EnergyChargeRate = FP.FromFloat_UNSAFE(energyChargeRate);
        _isDirty = true;
    }

    public void SetEnergyConsumptionRate(float energyConsumptionRate)
    {
        _currentRuleset.value.Players.EnergyConsumptionRate = FP.FromFloat_UNSAFE(energyConsumptionRate);
        _isDirty = true;
    }

    public void SetRespawnTime(string respawnTime)
    {
        _currentRuleset.value.Players.RespawnTime = Convert.ToInt32(respawnTime);
        _isDirty = true;
    }

    public void SetAllowFriendlyFire(bool allowFriendlyFire)
    {
        _currentRuleset.value.Players.AllowFriendlyFire = allowFriendlyFire;
        _isDirty = true;
    }

    public void SetAllowDuplicateSelectionBuilds(bool allowDuplicateSelection)
    {
        _currentRuleset.value.Players.AllowDuplicateSelection = allowDuplicateSelection;
        _isDirty = true;
    }

    public unsafe void ToggleStageAvailability(SerializableWrapper<Stage> stage)
    {
        List<AssetRefStageAsset> stages = ArrayHelper.All(_currentRuleset.value.Stage.Stages);

        if (stages.Contains(new() { Id = stage.FileID }))
        {
            // The stage is in the list and needs to be removed.
            ArrayHelper.Remove(ref _currentRuleset.value.Stage.Stages, new() { Id = stage.FileID });
        }
        else
        {
            // The stage is not in the list and needs to be added.
            ArrayHelper.Add(ref _currentRuleset.value.Stage.Stages, new() { Id = stage.FileID });
        }

        _isDirty = true;
    }

    public void SetStagePicker(StagePickerAsset stagePicker)
    {
        _currentRuleset.value.Stage.StagePicker = new() { Id = stagePicker.AssetObject.Guid };
        _isDirty = true;
    }

    public void SetAllowGizmos(bool allowGizmos)
    {
        _currentRuleset.value.Stage.AllowGizmos = allowGizmos;
        _isDirty = true;
    }

    public void SetAllowCustomStages(bool allowCustomStages)
    {
        _currentRuleset.value.Stage.AllowCustomStages = allowCustomStages;
        _isDirty = true;
    }

    public void SetAllowDuplicateSelectionStages(bool allowDuplicateSelection)
    {
        _currentRuleset.value.Stage.AllowDuplicateSelection = allowDuplicateSelection;
        _isDirty = true;
    }

    public void SetStartingItem(ItemAsset item)
    {
        _currentRuleset.value.Items.StartingItem = new() { Id = item.AssetObject.Guid };
        _isDirty = true;
    }

    public unsafe void ToggleItemAvailability(ItemAsset item)
    {
        List<AssetRefItem> items = ArrayHelper.All(_currentRuleset.value.Items.Items);

        if (items.Contains(new() { Id = item.AssetObject.Guid }))
        {
            // The item is in the list and needs to be removed.
            ArrayHelper.Remove(ref _currentRuleset.value.Items.Items, new() { Id = item.AssetObject.Guid });
        }
        else
        {
            // The item is not in the list and needs to be added.
            ArrayHelper.Add(ref _currentRuleset.value.Items.Items, new() { Id = item.AssetObject.Guid });
        }

        _isDirty = true;
    }

    public void SetItemSpawnFrequency(float itemSpawnFrequency)
    {
        _currentRuleset.value.Items.SpawnFrequency = FP.FromFloat_UNSAFE(itemSpawnFrequency);
        _isDirty = true;
    }

    public void LoadFromAsset(RulesetAssetAsset ruleset)
    {
        Load(ruleset.Ruleset, ruleset.AssetObject.Guid);
    }

    public void LoadFromSerializable(SerializableWrapper<Ruleset> ruleset)
    {
        Load(ruleset.value, ruleset.FileID);
        SendToSimulation();
    }

    public void Load(Ruleset ruleset, AssetGuid? guid = null)
    {
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        _currentRuleset = new(ruleset, GetPath(), "", "", guid.GetValueOrDefault(AssetGuid.NewGuid()), filterTags, groupTags);

        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStart.AddListener(_ => SendToSimulation());
    }

    public void ResetValue()
    {
        _currentRuleset = _none.Ruleset;
    }

    public void SendToSimulation()
    {
        CommandSetRuleset setRuleset = new()
        {
            ruleset = _currentRuleset
        };

        QuantumRunner.Default.Game.SendCommand(setRuleset);
    }
}
