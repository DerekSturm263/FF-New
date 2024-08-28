﻿
namespace Quantum
{
    [System.Serializable]
    public unsafe partial class StatusEffectSubEnhancer : SubEnhancer
    {
        public AssetRefStatusEffect StatusEffect;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, Sub sub)
        {
            if (f.Unsafe.TryGetPointer(target, out Stats* stats))
                StatsSystem.GiveStatusEffect(f, StatusEffect, target, stats);
        }
    }
}