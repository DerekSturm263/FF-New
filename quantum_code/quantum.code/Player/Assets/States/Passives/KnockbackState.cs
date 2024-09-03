using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockbackState : PlayerState
    {
        public FP MaximumYVelocity;
        public int MaxInfluence;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) && !filter.CharacterController->CurrentKnockback.Equals(default(KnockbackInfo)) && filter.CharacterController->CurrentKnockback.SetState;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->GetNearbyCollider(Colliders.LeftWall | Colliders.RightWall))
            {
                filter.CharacterController->CurrentKnockback.Direction.X *= -1;
            }

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "KnockbackY", filter.PhysicsBody->Velocity.Normalized.Y);
        }

        protected override FP GetMovementInfluence(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => FPMath.Clamp(filter.CharacterController->StateTime / MaxInfluence, FP._0, FP._1);

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.PhysicsBody->Velocity.Y < MaximumYVelocity || filter.CharacterController->GetNearbyCollider(Colliders.Ground);
        }
    }
}
