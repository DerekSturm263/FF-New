using Photon.Deterministic;

namespace Quantum
{
    public static class MainWeaponHelper
    {
        public static MainWeaponStats Add(MainWeaponStats lhs, MainWeaponStats rhs)
        {
            return new()
            {
                Damage = lhs.Damage + rhs.Damage,
                Knockback = lhs.Knockback + rhs.Knockback,
                Speed = lhs.Speed + rhs.Speed
            };
        }

        public static MainWeaponStats Multiply(MainWeaponStats lhs, MainWeaponStats rhs)
        {
            return new()
            {
                Damage = lhs.Damage * rhs.Damage,
                Knockback = lhs.Knockback * rhs.Knockback,
                Speed = lhs.Speed * rhs.Speed
            };
        }

        public static MainWeaponStats FromMainWeapon(Frame f, MainWeapon mainWeapon)
        {
            MainWeaponStats result = default;

            if (f.TryFindAsset(mainWeapon.Material.Id, out MainWeaponMaterial material1))
                result = Add(result, material1.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out MainWeaponMaterial material2))
                result = Add(result, material2.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out MainWeaponMaterial material3))
                result = Add(result, material3.Stats);

            if (f.TryFindAsset(mainWeapon.Material.Id, out MainWeaponMaterial material4))
                result = Add(result, material4.Stats);

            return result;
        }

        public static MainWeaponStats Default = new()
        {
            Damage = 1,
            Knockback = 1,
            Speed = 1
        };
    }
}
