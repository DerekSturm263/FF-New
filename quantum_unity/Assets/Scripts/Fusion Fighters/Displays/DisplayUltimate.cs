using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = UltimateAsset;

public class DisplayUltimate : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    //protected override Type GetValue() => new() { Settings = new Ultimate() { Guid = QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Ultimate.Id } };
    protected override Type GetValue() => default;
}
