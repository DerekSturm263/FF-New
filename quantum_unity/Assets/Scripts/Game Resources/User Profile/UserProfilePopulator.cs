using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = SerializableWrapper<UserProfile>;

public class UserProfilePopulator : Populate<Type, long>
{
    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => FusionFighters.Serializer.LoadAllFromDirectory<Type>(UserProfileController.GetPath());

    protected override string Name(Type item) => item.Name;

    protected override Func<Type, long> Sort() => (profile) => profile.LastEditedDate;

    protected override bool DoSpawn(Type item) => !PlayerJoinController.Instance.AllPlayers.Values.Any(user => user.Profile.Equals(item));
}
