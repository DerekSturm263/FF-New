﻿using Photon.Deterministic;

namespace Quantum
{
    public unsafe abstract partial class SubWeaponTemplate
    {
        public AssetRefEntityPrototype Prototype;
        public FP EnergyAmount;

        public virtual void OnSpawn(Frame f, EntityRef user, EntityRef subWeapon, SubWeaponInstance* subWeaponInstance) { }
        public virtual void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubWeaponInstance* subWeaponInstance) { }
        public virtual void OnDespawn(Frame f, EntityRef user, EntityRef subWeapon, SubWeaponInstance* subWeaponInstance) { }
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubWeaponInstance* subWeaponInstance) { }
    }
}