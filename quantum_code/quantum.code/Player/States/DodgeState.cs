using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class DodgeState : PlayerState
    {
        public override States GetState() => States.IsDodging;

        public override bool GetInput(ref Input input) => input.Dodge;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => filter.CharacterController->GetDodgeSettings(settings).Frames;
        protected override int DelayedEntranceTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.DirectionChangeTime;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting, States.IsCrouching, States.IsUsingAltWeapon, States.IsInteracting, States.IsUsingMainWeapon, States.IsUsingUltimate };
        public override States[] KillStateList => new States[] { States.IsJumping };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.CharacterController->DodgeCount > 0;
        }

        protected override bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !filter.CharacterController->GroundedDodge && filter.CharacterController->GetNearbyCollider(Colliders.Ground);
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            bool isGrounded = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            if (isGrounded)
                --filter.CharacterController->JumpCount;

            filter.CharacterController->GroundedDodge = isGrounded;

            filter.PhysicsBody->GravityScale = 0;

            --filter.CharacterController->DodgeCount;
            filter.CharacterController->DodgeTime = 0;
        }

        protected override void DelayedEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.DelayedEnter(f, ref filter, ref input, settings, stats);
            
            filter.CharacterController->DodgeDirection = input.SnapMovementTo8Directions;
            if (filter.CharacterController->GroundedDodge)
                filter.CharacterController->DodgeDirection.Y = 0;

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirX", filter.CharacterController->DodgeDirection.X);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirY", filter.CharacterController->DodgeDirection.Y);

            if (filter.CharacterController->DodgeDirection.X != 0)
            {
                filter.CharacterController->MovementDirection = -FPMath.Sign(filter.CharacterController->DodgeDirection.X).AsInt;
                f.Events.OnPlayerChangeDirection(filter.Entity, filter.Stats->PlayerIndex, filter.CharacterController->MovementDirection);
            }
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Update(f, ref filter, ref input, settings, stats);

            filter.CharacterController->DodgeTime++;

            if (filter.CharacterController->DodgeTime > DelayedEntranceTime(f, ref filter, ref input, settings, stats))
            {
                MovementCurveSettings dodgeSettings = filter.CharacterController->GetDodgeSettings(settings);

                FPVector2 dodge = (filter.CharacterController->DodgeDirection * dodgeSettings.Curve.Evaluate(filter.CharacterController->DodgeTime) * dodgeSettings.Force);
                filter.CharacterController->Velocity = dodge.X;
                filter.PhysicsBody->Velocity.Y = dodge.Y;
            }
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() {  CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DisableHitbox = false });

            filter.PhysicsBody->GravityScale = 1;
            filter.CharacterController->DodgeTime = 0;
        }
    }
}
