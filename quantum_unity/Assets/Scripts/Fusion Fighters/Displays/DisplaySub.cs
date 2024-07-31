using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Sub>;

public class DisplaySub : DisplayTextAndImage<Type>
{
    [SerializeField] private string _fontSize = "50";

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (!item.Equals(default(Type)))
            return new(string.Format(_format, item.Name, item.Description), item.Icon);
        else
            return new("None", null);
    }

    protected override Type GetValue()
    {
        AssetGuid id = BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.SubWeapon.FileGuid;

        if (id.IsValid)
            return Type.LoadAs(WeaponController.GetPath(), id);
        else
            return default;
    }

    public void Clear()
    {
        _component.Item1.Invoke("(Empty)");
        _component.Item2.Invoke(null);
    }

    public void DisplayEmpty()
    {
        _component.Item1.Invoke($"<font=\"KeaniaOne-Title SDF\"><size={_fontSize}>None</size></font>");
        _component.Item2.Invoke(null);
    }
}
