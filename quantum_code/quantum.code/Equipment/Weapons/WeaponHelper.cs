﻿using Photon.Deterministic;

namespace Quantum
{
    public static class WeaponHelper
    {
        private static WeaponStats Add(WeaponStats lhs, WeaponStats rhs)
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

        public static unsafe WeaponStats FromStats(Frame f, Stats* stats)
        {
            WeaponStats weaponStats = Default;

            weaponStats = Add(FromWeapon(f, stats->Build.Equipment.Weapons.MainWeapon), weaponStats);

            return Multiply(weaponStats, stats->WeaponStatsMultiplier);
        }

        private static WeaponStats FromWeapon(Frame f, Weapon mainWeapon)
        {
            WeaponStats result = default;

            if (f.TryFindAsset(mainWeapon.Material.Id, out WeaponMaterial material))
                result = Add(result, material.Stats);

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
