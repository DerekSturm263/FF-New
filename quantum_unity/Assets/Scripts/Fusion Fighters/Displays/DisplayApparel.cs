using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Apparel>;

public class DisplayApparel : DisplayTextAndImage<Type>
{
    [SerializeField] private ApparelTemplate.ApparelType _type;

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.Name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => _type switch
    {
        ApparelTemplate.ApparelType.Headgear => new(QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Outfit.Headgear, "", "", AssetGuid.NewGuid(), 0, 0),
        ApparelTemplate.ApparelType.Clothing => new(QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Outfit.Clothing, "", "", AssetGuid.NewGuid(), 0, 0),
        ApparelTemplate.ApparelType.Legwear => new(QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Outfit.Legwear, "", "", AssetGuid.NewGuid(), 0, 0),
        _ => default,
    };
}
