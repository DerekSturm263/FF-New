namespace Quantum
{
    [System.Serializable]
    public unsafe abstract partial class WeaponEnhancer : InfoAsset
    {
        public virtual void OnMoveStart(Frame f, ref CharacterControllerSystem.Filter filter, ref HitboxSettings hitboxSettings) { }
        public virtual void OnMoveUpdate(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public virtual void OnMoveEnd(Frame f, ref CharacterControllerSystem.Filter filter) { }
        public virtual void OnHit(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef target, HitboxSettings hitbox) { }
    }
}
