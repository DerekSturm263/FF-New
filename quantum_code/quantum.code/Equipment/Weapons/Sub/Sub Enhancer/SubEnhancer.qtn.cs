namespace Quantum
{
    [System.Serializable]
    public unsafe abstract partial class SubEnhancer : InfoAsset
    {
        public virtual void OnApply(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public virtual void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter) { }

        public virtual void OnSpawn(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef subWeapon, Sub sub) { }
        public virtual void OnUpdate(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef target, EntityRef subWeapon, Sub sub) { }
        public virtual void OnDespawn(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef subWeapon, Sub sub) { }
        public virtual void OnHit(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef target, EntityRef subWeapon, Sub sub) { }
    }
}
