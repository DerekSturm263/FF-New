using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = ApparelModifierAsset;

public class DisplayApparelModifierInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => default;

    public void UpdateFromIndex(int index)
    {
        var list = ApparelController.Instance.GetModifierList();
        Tuple<string, Sprite> values = GetInfo(ApparelController.Instance.GetModifierFromIndex(list, index));

        _component.Item1.Invoke(values.Item1);
        _component.Item2.Invoke(values.Item2);
    }
}
