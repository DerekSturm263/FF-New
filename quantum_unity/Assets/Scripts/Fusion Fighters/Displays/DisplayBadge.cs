using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = BadgeAsset;

public class DisplayBadge : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    //protected override Type GetValue() => new() { Settings = new Badge() { Guid = QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Badge.Id } };
    protected override Type GetValue() => default;
}
