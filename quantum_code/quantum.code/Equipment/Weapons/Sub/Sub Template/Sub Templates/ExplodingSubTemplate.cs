namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingSubTemplate : SubTemplate
    {
        public HitboxSettings HitboxSettings;
        public int Lifetime;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubInstance* subWeaponInstance)
        {
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Lifetime, user);
        }
    }
}
