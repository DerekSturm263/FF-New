using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using Quantum;
using System.Linq;
using UnityEngine;

public class UserProfileController : Controller<UserProfileController>
{
    [SerializeField] private PopulateBase _populator;

    public static string GetPath() => $"{Application.persistentDataPath}/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new();
        return new(profile, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<UserProfile> profile)
    {
        Serializer.Save(profile, profile.Guid, GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(UserProfilePopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }
    
    public void SetOnPlayer(SerializableWrapper<UserProfile> profile, int localIndex)
    {
        PlayerJoinController.Instance.AllPlayers.ElementAt(localIndex).Value.SetProfile(profile.Value);
    }
}
