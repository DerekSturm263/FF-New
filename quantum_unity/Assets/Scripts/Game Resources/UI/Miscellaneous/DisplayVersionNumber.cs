using UnityEngine;

namespace GameResources.UI.Miscellaneous
{
    using Type = Application;

    public class DisplayVersionNumber : Extensions.Components.UI.DisplayText<Type>
    {
        protected override Type GetValue() => default;

        protected override string GetInfo(Type item) => Type.version;
    }
}
