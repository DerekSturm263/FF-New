using Quantum;
using UnityEngine;

public class UserProfileController : SpawnableController<UserProfile>
{
    [SerializeField] private Sprite _profileIcon;

    private LocalPlayerInfo _player;
    public void SetPlayer(LocalPlayerInfo player) => _player = player;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new(_hapticStrength);
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        return new(profile, _name, "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        profile.Save(GetPath());
    }

    public void SaveNew()
    {
        UserProfile profile = new(_hapticStrength);
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        SerializableWrapper<UserProfile> serialized = new(profile, _name, "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);
        serialized.Save(GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => (Instance as UserProfileController).Delete();

    private void Delete()
    {
        _currentlySelected.Delete(GetPath());

        if (UserProfilePopulator.Instance && UserProfilePopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
            Destroy(button);
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

    private System.Action _action;

    public void DeferEvents(System.Action action)
    {
        _action = action;
    }

    public void ExecuteDeferredEvents()
    {
        _action.Invoke();
    }

    public void Cancel()
    {
        PlayerJoinController.Instance.RemovePlayer(_player);
    }
}
