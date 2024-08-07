using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
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

        _currentRuleset = new(ruleset, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);
        _isDirty = true;
    }

    public void Save(SerializableWrapper<Ruleset> ruleset)
    {
        ruleset.Save(GetPath());
    }

    public void SaveCurrent()
    {
        _currentRuleset.Save(GetPath());
        _isDirty = false;
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
        _currentRuleset.Delete(GetPath());

        if (RulesetPopulator.Instance && RulesetPopulator.Instance.TryGetButtonFromItem(_currentRuleset, out GameObject button))
            Destroy(button);

        FindFirstObjectByType<RulesetPopulator>()?.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void LoadFromAsset(RulesetAssetAsset ruleset)
    {
        Load(ruleset.Ruleset);
    }

    public void Load(Ruleset ruleset)
    {
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        _currentRuleset = new(ruleset, "", "", 0, 0, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);

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
