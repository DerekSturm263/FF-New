using UnityEngine;
using FusionFighters.Profile;

public class ProfileController : SpawnableController<Profile>
{
    [SerializeField] private ProfileAsset _default;

    private Profile _profile;
    public Profile Profile => _profile;

    [System.NonSerialized] private bool _isInitialized = false;

    protected override bool TakeAwayFocus() => true;

    public void Spawn()
    {
        Spawn(default);
    }

    protected override void SetUp(Profile t)
    {

    }

    public override void Initialize()
    {
        base.Initialize();

        if (!_isInitialized)
        {
            if (FusionFighters.Serializer.TryLoadAs($"{Application.persistentDataPath}/SaveData/Misc/Profile.json", $"{Application.persistentDataPath}/SaveData/Misc", out Profile profile))
                _profile = profile.DeepCopy();
            else
                _profile = _default.Profile.DeepCopy();

            Application.quitting += Shutdown;
            _isInitialized = true;
        }
    }

    public override void Shutdown()
    {
        Application.quitting -= Shutdown;
        _isInitialized = false;

        FusionFighters.Serializer.Save(_profile, "Profile", $"{Application.persistentDataPath}/SaveData/Misc");

        base.Shutdown();
    }

    public void SetName(string name)
    {
        _profile.SetUsername(name);
        FindFirstObjectByType<DisplayProfile>().UpdateDisplay();
    }
}
