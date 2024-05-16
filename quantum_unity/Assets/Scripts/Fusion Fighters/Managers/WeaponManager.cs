using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class WeaponManager : Controller<WeaponManager>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Weapons";

    public override void Initialize()
    {
        base.Initialize();
    }

    public Weapon New()
    {
        Weapon weapon = new();

        weapon.SerializableData.Name = "Untitled";
        weapon.SerializableData.Guid = AssetGuid.NewGuid();
        weapon.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        weapon.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return weapon;
    }

    public void Save(Weapon weapon)
    {
        Serializer.Save(weapon, weapon.SerializableData.Guid, GetPath());
    }
}
