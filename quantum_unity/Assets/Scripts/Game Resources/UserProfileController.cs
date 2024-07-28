using Quantum;
using UnityEngine;

public class UserProfileController : SpawnableController<UserProfile>
{
    private LocalPlayerInfo _player;
    public void SetPlayer(LocalPlayerInfo player) => _player = player;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new(_hapticStrength);

        return new(profile, _name, "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid());
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        profile.Save(GetPath());
    }

    public void SaveNew()
    {
        UserProfile profile = new(_hapticStrength);
        SerializableWrapper<UserProfile> serialized = new(profile, _name, "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid());

        serialized.Save(GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => (Instance as UserProfileController).Delete();

    private void Delete()
    {
        _currentlySelected.Delete(GetPath());

        Destroy(UserProfilePopulator.ButtonFromItem(_currentlySelected));
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
