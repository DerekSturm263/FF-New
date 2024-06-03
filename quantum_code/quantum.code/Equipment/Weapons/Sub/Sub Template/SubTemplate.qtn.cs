using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SubTemplate : InfoAsset
    {
        public AssetRefItem Prototype;
        public FP EnergyAmount;

        public virtual void OnSpawn(Frame f, EntityRef user, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnDespawn(Frame f, EntityRef user, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubInstance* subWeaponInstance) { }
    }
}
