using System;
using System.Collections.Generic;
using System.Linq;

public class UserProfilePopulator : PopulateSerializable<UserProfile, UserProfileAsset>
{
    protected override string BuiltInFilePath() => "Scriptable Objects/User Profiles";
    protected override string CustomFilePath() => UserProfileController.GetPath();

    protected override SerializableWrapper<UserProfile> GetFromBuiltInAsset(UserProfileAsset asset) => asset.Profile;

    protected override Dictionary<string, Predicate<SerializableWrapper<UserProfile>>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (value) => !IsEquipped(value) || IsNone(value)
        };
    }
    protected override Predicate<SerializableWrapper<UserProfile>> GetDefaultFilterMode() => _allFilterModes["All"];

    protected override bool IsEquipped(SerializableWrapper<UserProfile> item) => PlayerJoinController.Instance.LocalPlayers.Any(user => user.Profile.Equals(item));
    protected override bool IsNone(SerializableWrapper<UserProfile> item) => false;
}
