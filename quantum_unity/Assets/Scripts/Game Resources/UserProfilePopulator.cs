using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserProfilePopulator : PopulateSerializable<UserProfile, UserProfileAsset>
{
    public static UserProfilePopulator RealInstance;

    [SerializeField] private bool _isInstance;

    protected override void Awake()
    {
        base.Awake();

        if (_isInstance)
            RealInstance = this;
    }

    protected override Sprite Icon(SerializableWrapper<UserProfile> item) => item.value.LastBuild.Icon;

    protected override string BuiltInFilePath() => "DB/Assets/User Profiles";
    protected override string CustomFilePath() => UserProfileController.GetPath();

    protected override SerializableWrapper<UserProfile> GetFromBuiltInAsset(UserProfileAsset asset)
    {
        var item = asset.Profile;

        return item;
    }

    protected override Dictionary<string, Predicate<SerializableWrapper<UserProfile>>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (value) => !IsEquipped(value)
        };
    }

    protected override bool IsEquipped(SerializableWrapper<UserProfile> item) => PlayerJoinController.Instance.GetAllLocalPlayers(false).Any(user => user.Profile.Equals(item));
    protected override bool IsNone(SerializableWrapper<UserProfile> item) => !item.IsValid;
}
