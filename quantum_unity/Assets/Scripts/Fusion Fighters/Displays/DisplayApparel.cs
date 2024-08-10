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
        if (item.IsValid)
            return new(string.Format(_format, item.Name, item.Description), item.Icon);
        else
            return new(string.Format(_format, "None", ""), null);
    }

    protected override Type GetValue()
    {
        AssetGuid id = _type switch
        {
            ApparelTemplate.ApparelType.Headgear => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Headgear.FileGuid,
            ApparelTemplate.ApparelType.Clothing => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Clothing.FileGuid,
            ApparelTemplate.ApparelType.Legwear => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Legwear.FileGuid,
            _ => default
        };

        if (id.IsValid)
            return Type.LoadAs(ApparelController.GetPath(), id);
        else
            return default;
    }

    public void Clear()
    {
        _component.Item1.Invoke(string.Format(_format, "None", ""));
        _component.Item2.Invoke(null);
    }

    public void DisplayEmpty()
    {
        _component.Item1.Invoke(string.Format(_format, "", ""));
        _component.Item2.Invoke(null);
    }
}
