using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class StageController : Controller<StageController>
{
    [SerializeField] private StageAssetAsset _default;
    [SerializeField] private StageAssetAsset _none;

    [SerializeField] private Popup _savePopup;

    private bool _isDirty;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Stages";

    private SerializableWrapper<Stage> _currentStage;
    public SerializableWrapper<Stage> CurrentStage => _currentStage;
    public void Select(SerializableWrapper<Stage> stage, FighterIndex index)
    {
        _currentStage = stage;
        _isDirty = false;
    }

    public override void Initialize()
    {
        base.Initialize();

        _currentStage = _none.Stage;
    }

    public void New()
    {
        Stage stage = _default.Stage;
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        _currentStage = new(stage, GetPath(), "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags);
        _isDirty = true;
    }

    public void Save(SerializableWrapper<Stage> stage)
    {
        stage.Save();
    }

    public void SaveCurrent()
    {
        _currentStage.Save();
        _isDirty = false;

        ToastController.Instance.Spawn("Stage saved");
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
        _currentStage.SetName(name);
        _isDirty = true;
    }

    public void SetDescription(string description)
    {
        _currentStage.SetDescription(description);
        _isDirty = true;
    }

    public void Delete()
    {
        _currentStage.Delete();

        if (StagePopulator.Instance && StagePopulator.Instance.TryGetButtonFromItem(_currentStage, out GameObject button))
            Destroy(button);

        FindFirstObjectByType<StagePopulator>()?.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);

        ToastController.Instance.Spawn("Stage deleted");
    }

    public void LoadFromAsset(StageAssetAsset stage)
    {
        Load(stage.Stage);
    }

    public void Load(Stage stage)
    {
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        _currentStage = new(stage, GetPath(), "", "", 0, 0, AssetGuid.NewGuid(), filterTags, groupTags);

        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStart.AddListener(_ => SendToSimulation());
    }

    public void ResetValue()
    {
        _currentStage = _none.Stage;
    }

    public void SendToSimulation()
    {
        CommandSetStage setStage = new()
        {
            stage = _currentStage
        };

        QuantumRunner.Default.Game.SendCommand(setStage);
    }

    public void UpdateDisplay(SerializableWrapper<Stage> stage)
    {
        FindFirstObjectByType<DisplayStageInfo>().UpdateDisplay(stage);
    }
}
