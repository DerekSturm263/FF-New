using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class JumpState : PlayerState
    {
        public override States GetState() => States.IsJumping;

        public override bool GetInput(ref Input input) => input.Jump;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => filter.CharacterController->GetJumpSettings(settings).Frames;
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;
        public override bool CanInterruptSelf => true;

        public override States[] EntranceBlacklist => new States[] { States.IsBlocking, States.IsDodging, States.IsInteracting, States.IsUsingUltimate };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.CharacterController->JumpCount > 0;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->IsInState(States.IsJumping))
                CustomAnimator.SetTrigger(f, filter.CustomAnimator, "Double Jump");

            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->GroundedJump = filter.CharacterController->GetNearbyCollider(Colliders.Ground);

            f.Events.OnPlayerJump(filter.Entity, filter.Stats->Index, stats.Jump.AsInt - filter.CharacterController->JumpCount);

            --filter.CharacterController->JumpCount;
            filter.CharacterController->JumpTime = 0;
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);

            if (filter.CharacterController->GroundedJump && (!input.Jump || input.MovementDown))
                filter.CharacterController->JumpSettingsIndex = 0;
            else
                filter.CharacterController->JumpSettingsIndex = 1;
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Update(f, ref filter, ref input, settings, stats);

            filter.CharacterController->JumpTime++;

            if (filter.CharacterController->JumpTime > DelayedEntranceTime(f, ref filter, ref input, settings, stats))
            {
                MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(settings);
                filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->JumpTime) * (jumpSettings.Force * (1 / stats.Weight));
            }
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            filter.CharacterController->JumpSettingsIndex = 0;
            filter.CharacterController->JumpTime = 0;
        }
    }
}
