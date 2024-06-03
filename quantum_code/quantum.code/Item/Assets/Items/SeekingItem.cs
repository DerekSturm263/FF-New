using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SeekingItem : UpdateableItem
    {
        public FP Speed;
        public FP LerpTime;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            itemInstance->Target = StatsSystem.FindNearestOtherPlayer(f, user);
        }

        public override unsafe void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody) &&
                f.Unsafe.TryGetPointer(item, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(target, out Transform2D* targetTransform))
            {
                physicsBody->Velocity = FPVector2.Lerp(physicsBody->Velocity, (targetTransform->Position - transform->Position).Normalized * Speed, f.DeltaTime * LerpTime);
            }
        }
    }
}
