using UnityEngine;

namespace FusionFighters.Profile
{
    using Type = Profile;

    public class DisplayProfile : Extensions.Components.UI.DisplayTextAndImage<Type>
    {
        protected override Type GetValue() => (ProfileController.Instance as ProfileController).Profile;

        protected override Extensions.Types.Tuple<string, Sprite> GetInfo(Type item)
        {
            return new(item.Username, item.Icon);
        }
    }
}