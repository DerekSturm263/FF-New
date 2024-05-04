namespace Quantum
{
    public unsafe class SubWeaponSystem : SystemMainThreadFilter<SubWeaponSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public SubWeaponInstance* SubWeaponInstance;
            public ItemInstance* ItemInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            SubWeaponTemplate template = f.FindAsset<SubWeaponTemplate>(filter.SubWeaponInstance->SubWeapon.Template.Id);
            template.OnUpdate(f, filter.ItemInstance->Owner, filter.SubWeaponInstance->Target, filter.Entity);
        }

        public static EntityRef Spawn(Frame f, SubWeapon subWeapon, PlayerLink* player)
        {
            SubWeaponTemplate template = f.FindAsset<SubWeaponTemplate>(subWeapon.Template.Id);
            EntityRef entity = f.Create(f.FindAsset<EntityPrototype>(template.Prototype.Id));

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                if (f.Unsafe.TryGetPointer(player->Entity, out Stats* stats))
                {
                    var dict = f.ResolveDictionary(stats->Hurtboxes);

                    if (f.Unsafe.TryGetPointer(dict[HurtboxType.RightHand], out Transform2D* parentTransform))
                    {
                        transform->Position = parentTransform->Position;
                    }

                    if (f.Unsafe.TryGetPointer(entity, out ItemInstance* itemInstance))
                        itemInstance->Owner = player->Entity;

                    template.OnSpawn(f, player->Entity, entity);
                }
            }

            if (f.Unsafe.TryGetPointer(entity, out SubWeaponInstance* subWeaponInstance))
            {
                subWeaponInstance->SubWeapon = subWeapon;
            }

            return entity;
        }
    }
}
