using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe abstract partial class SubTemplate : InfoAsset
    {
        public AssetRefEntityPrototype Prototype;
        public FP EnergyAmount;

        public virtual void OnSpawn(Frame f, EntityRef user, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnDespawn(Frame f, EntityRef user, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
    }
}
