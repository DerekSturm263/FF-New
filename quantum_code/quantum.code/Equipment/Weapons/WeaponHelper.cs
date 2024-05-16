using Photon.Deterministic;

namespace Quantum
{
    public static class WeaponHelper
    {
        public static WeaponStats Add(WeaponStats lhs, WeaponStats rhs)
        {
            return new()
            {
                Damage = lhs.Damage + rhs.Damage,
                Knockback = lhs.Knockback + rhs.Knockback,
                Speed = lhs.Speed + rhs.Speed
            };
        }

        public static WeaponStats Multiply(WeaponStats lhs, WeaponStats rhs)
        {
            return new()
            {
                Damage = lhs.Damage * rhs.Damage,
                Knockback = lhs.Knockback * rhs.Knockback,
                Speed = lhs.Speed * rhs.Speed
            };
        }

        public static WeaponStats FromMainWeapon(Frame f, Weapon mainWeapon)
        {
            WeaponStats result = default;

            if (f.TryFindAsset(mainWeapon.Material.Id, out WeaponMaterial material1))
                result = Add(result, material1.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out WeaponMaterial material2))
                result = Add(result, material2.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out WeaponMaterial material3))
                result = Add(result, material3.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out WeaponMaterial material4))
                result = Add(result, material4.Stats);

            return result;
        }

        public static WeaponStats Default = new()
        {
            Damage = 1,
            Knockback = 1,
            Speed = 1
        };
    }
}
