namespace Quantum
{
    [System.Serializable]
    public unsafe abstract partial class SubEnhancer : InfoAsset
    {
        public virtual void OnApply(Frame f, EntityRef user) { }
        public virtual void OnRemove(Frame f, EntityRef user) { }

        public virtual void OnSpawn(Frame f, EntityRef user, EntityRef subWeapon, Sub sub) { }
        public virtual void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, Sub sub) { }
        public virtual void OnDespawn(Frame f, EntityRef user, EntityRef subWeapon, Sub sub) { }
        public virtual void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, Sub sub) { }
    }
}
