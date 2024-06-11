using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using Quantum;
using UnityEngine;

public class UserProfileController : Controller<UserProfileController>
{
    [SerializeField] private PopulateBase _populator;

    public static string GetPath() => $"{Application.persistentDataPath}/Profiles";

    public SerializableWrapper<UserProfile> New()
    {
        UserProfile profile = new();

        profile.SerializableData.Name = "Untitled";
        profile.SerializableData.Guid = AssetGuid.NewGuid();
        profile.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        profile.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return new(profile);
    }

    public void Save(SerializableWrapper<UserProfile> build)
    {
        Serializer.Save(build, build.Value.SerializableData.Guid, GetPath());
    }

    private SerializableWrapper<UserProfile> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<UserProfile> profile) => _currentlySelected = profile;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Value.SerializableData.Guid}.json", path);

        Destroy(UserProfilePopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }
}
