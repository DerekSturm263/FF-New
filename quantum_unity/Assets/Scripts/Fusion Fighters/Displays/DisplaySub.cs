using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Sub>;

public class DisplaySub : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (!item.Equals(default(Type)))
            return new(string.Format(_format, item.Name, item.Description), item.Icon);
        else
            return new(string.Format(_format, "None", ""), null);
    }

    protected override Type GetValue()
    {
        AssetGuid id = BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.SubWeapon.FileGuid;

        if (id.IsValid)
            return Type.LoadAs(SubController.GetPath(), id);
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
