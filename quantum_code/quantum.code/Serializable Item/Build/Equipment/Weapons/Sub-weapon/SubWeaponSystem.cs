namespace Quantum
{
        public unsafe class SubWeaponSystem : SystemMainThreadFilter<SubWeaponSystem.Filter>
        {
                public struct Filter
                {
                        public EntityRef Entity;
                        public SubWeaponInstance* SubWeaponInstance;
                }

                public override void Update(Frame f, ref Filter filter)
                {
                        SubWeaponTemplate template = f.FindAsset<SubWeaponTemplate>(filter.SubWeaponInstance->SubWeapon.Template.Id);
                        template.OnUpdate(f, filter.SubWeaponInstance->User, filter.SubWeaponInstance->Target, filter.Entity);
                }
        }
}
