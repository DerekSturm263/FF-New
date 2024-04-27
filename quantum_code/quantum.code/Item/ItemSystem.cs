namespace Quantum
{
    public unsafe class ItemSystem : SystemMainThreadFilter<ItemSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public ItemInstance* ItemInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.ItemInstance->Holder.IsValid)
            {
                if (f.Unsafe.TryGetPointer(filter.ItemInstance->Holder, out Stats* stats))
                {
                    var dict = f.ResolveDictionary(stats->Hurtboxes);

                    if (f.Unsafe.TryGetPointer(dict[HurtboxType.RightHand], out Transform2D* transform))
                    {
                        filter.Transform->Position = transform->Position;
                    }
                }
            }
        }
    }
}
