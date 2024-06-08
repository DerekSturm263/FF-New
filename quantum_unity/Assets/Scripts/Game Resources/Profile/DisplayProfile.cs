using UnityEngine;

namespace FusionFighters.Profile
{
    using Type = Profile;

    public class DisplayProfile : Extensions.Components.UI.DisplayTextAndImage<Type>
    {
        protected override Type GetValue() => Type.Instance;

        protected override Extensions.Types.Tuple<string, Sprite> GetInfo(Type item)
        {
            if (!SteamManager.Initialized)
                return new(string.Empty, null);

            return new(item.Username, Sprite.Create(item.Icon, new(0, 0, 184, -184), Vector2.one));
        }
    }
}