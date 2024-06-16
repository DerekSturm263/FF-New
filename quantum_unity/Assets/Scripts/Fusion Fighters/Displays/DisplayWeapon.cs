using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Weapon>;

public class DisplayWeapon : DisplayTextAndImage<Type>
{
    [SerializeField] private string _fontSize = "50";

    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size={_fontSize}>{item.Name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => new(QuantumRunner.Default.Game.Frames.Verified.Get<Stats>(BuildController.Instance.GetPlayerLocalIndex(0)).Build.Equipment.Weapons.MainWeapon, "", "", AssetGuid.NewGuid(), 0, 0);

    public void Clear()
    {
        _component.Item1.Invoke("(Empty)");
        _component.Item2.Invoke(null);
    }
}
