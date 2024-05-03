using Photon.Deterministic;

namespace Quantum
{
    public unsafe abstract partial class SubWeaponTemplate
    {
        public FP EnergyAmount;

        public abstract void OnSpawn(Frame f, EntityRef user, EntityRef subWeaponInstance);
        public virtual void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeaponInstance) { }
        public abstract void OnDespawn(Frame f, EntityRef user, EntityRef subWeaponInstance);
    }
}
