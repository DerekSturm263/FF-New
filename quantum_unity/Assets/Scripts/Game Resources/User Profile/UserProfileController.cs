using Quantum;
using System;
using UnityEngine;

public class UserProfileController : SpawnableController<UserProfile>
{
    private LocalPlayerInfo _player;
    public void SetPlayer(LocalPlayerInfo player) => _player = player;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new();
        return new(profile, _name, "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        Serializer.Save(profile, profile.Guid, GetPath());
    }

    public void SaveNew()
    {
        UserProfile profile = new();
        SerializableWrapper<UserProfile> serialized = new(profile, _name, "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);

        Serializer.Save(serialized, serialized.Guid, GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => (Instance as UserProfileController).Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(UserProfilePopulator.ButtonFromItem(_currentlySelected));
        //_populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void SetName(string name)
    {
        _currentlySelected.SetName(name);
    }

    private string _name;
    public void SetNameUnfinished(string name) => _name = name;

    public void SetOnPlayer(SerializableWrapper<UserProfile> profile)
    {
        _player.SetProfile(profile);
        FindFirstObjectByType<DisplayUsers>()?.UpdateDisplay();
    }

    private Action _action;

    public void DeferEvents(Action action)
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
