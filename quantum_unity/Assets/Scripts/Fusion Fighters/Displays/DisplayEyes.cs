using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = EyesAsset;

public class DisplayEyes : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => new() { Settings_Eyes = new Eyes() { Guid = QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Cosmetics.Eyes.Id } };
}
