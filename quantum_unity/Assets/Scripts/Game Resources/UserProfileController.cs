using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class UserProfileController : SpawnableController<bool>
{
    [SerializeField] private BuildAssetAsset _default;

    [SerializeField] private Sprite _profileIcon;

    private LocalPlayerInfo _player;
    public void SetPlayer(LocalPlayerInfo player) => _player = player;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Profiles";

    protected override bool TakeAwayFocus() => true;

    protected override void SetUp(bool isJoining)
    {
        _templateInstance.transform.GetChild(1).GetChild(1).gameObject.SetActive(isJoining);
        _templateInstance.transform.GetChild(1).GetChild(2).gameObject.SetActive(!isJoining);
    }

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new(_hapticStrength, _default.Build);
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        return new(profile, GetPath(), _name, "", AssetGuid.NewGuid(), filterTags, groupTags);
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        profile.Save();
    }

    public void SaveNew()
    {
        UserProfile profile = new(_hapticStrength, _default.Build);
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        SerializableWrapper<UserProfile> serialized = new(profile, GetPath(), _name, "", AssetGuid.NewGuid(), filterTags, groupTags);
        serialized.Save();
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => (Instance as UserProfileController).Delete();

    private void Delete()
    {
        _currentlySelected.Delete();

        if (UserProfilePopulator.Instance && UserProfilePopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
        {
            DestroyImmediate(button);
            UserProfilePopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }

        ToastController.Instance.Spawn("User Profile deleted");
    }

    public void SetName(string name)
    {
        _currentlySelected.SetName(name);
    }

    private string _name;
    public void SetNameUnfinished(string name) => _name = name;

    private float _hapticStrength;
    public void SetHapticStrengthUnfinished(float hapticStrength)
    {
        _hapticStrength = hapticStrength;
        PlayerJoinController.Instance.Rumble(_player, _hapticStrength, 0.1f);
    }

    public void SetProfile(SerializableWrapper<UserProfile> profile)
    {
        _player.SetProfile(profile);
    }

    public void UpdateDisplay()
    {
        FindFirstObjectByType<DisplayUsers>()?.UpdateDisplay();
    }

    private System.Action _joinEvents;
    public void DeferJoinEvents(System.Action action) => _joinEvents = action;
    public void ExecuteDeferredJoinEvents() => _joinEvents.Invoke();

    private System.Action _changeEvents;
    public void DeferChangeEvents(System.Action action) => _changeEvents = action;
    public void ExecuteDeferredChangeEvents() => _changeEvents.Invoke();

    private System.Action _leaveEvents;
    public void DeferLeaveEvents(System.Action action) => _leaveEvents = action;
    public void ExecuteDeferredLeaveEvents() => _leaveEvents.Invoke();

    public void Cancel()
    {
        PlayerJoinController.Instance.RemovePlayer(_player);
    }
}
