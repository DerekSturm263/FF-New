namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ExplodingSubWeaponTemplate : SubWeaponTemplate
    {
        public HitboxSettings HitboxSettings;
        public int Lifetime;

        public override void OnHit(Frame f, EntityRef user, EntityRef target, EntityRef subWeapon, SubWeaponInstance* subWeaponInstance)
        {
            HitboxSystem.SpawnHitbox(f, HitboxSettings, Lifetime, user);
        }
    }
}
