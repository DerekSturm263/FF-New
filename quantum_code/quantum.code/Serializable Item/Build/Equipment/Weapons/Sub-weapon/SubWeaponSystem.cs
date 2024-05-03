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
                        filter.SubWeaponInstance->
                }
        }
}
