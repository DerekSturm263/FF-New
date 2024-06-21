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
        if (item is not null)
            return new($"<font=\"KeaniaOne-Title SDF\"><size={_fontSize}>{item.Name}</size></font>\n\n{item.Description}", item.Icon);
        else
            return new("", null);
    }

    protected override Type GetValue()
    {
        AssetGuid id = BuildController.Instance.CurrentlySelected.Value.Equipment.Weapons.SubWeapon.FileGuid;

        if (id.IsValid)
            return Serializer.LoadAs<Type>($"{SubController.GetPath()}/{id}.json", SubController.GetPath());
        else
            return null;
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
