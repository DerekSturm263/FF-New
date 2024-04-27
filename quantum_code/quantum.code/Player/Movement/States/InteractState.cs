namespace Quantum.Movement
{
    public unsafe sealed class InteractState : PlayerState
    {
        public override States GetState() => States.IsInteracting;

        public override bool GetInput(ref Input input) => input.Interact;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => 1;

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            if (filter.CharacterController->HeldItem.IsValid)
            {
                if (f.Unsafe.TryGetPointer(filter.CharacterController->HeldItem, out ItemInstance* itemInstance))
                {
                    itemInstance->Holder = default;
                }
                
                if (f.Unsafe.TryGetPointer(filter.CharacterController->HeldItem, out PhysicsBody2D* physicsBody))
                {
                    physicsBody->AddForce(settings.ThrowAmount);
                }

                filter.CharacterController->HeldItem = default;
            }
            else
            {
                if (settings.InteractCast.TryGetCastResults(f, out Physics2D.HitCollection hitCollection, filter.Transform))
                {
                    if (f.Unsafe.TryGetPointer(hitCollection[0].Entity, out ItemInstance* itemInstance))
                    {
                        itemInstance->Holder = hitCollection[0].Entity;
                        filter.CharacterController->HeldItem = hitCollection[0].Entity;
                    }
                }
            }
        }
    }
}
