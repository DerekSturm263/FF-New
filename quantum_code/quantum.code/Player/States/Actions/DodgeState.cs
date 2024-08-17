using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class DodgeState : DirectionalActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Dodge;

        public override (States, StatesFlag) GetStateInfo() => (States.Dodge, StatesFlag.Dodge);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new() { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetDodgeSettings(settings).Frames;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                filter.CharacterController->DodgeCount > 0 &&
                input.Movement.Magnitude >= settings.DeadStickZone;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            bool isGrounded = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            if (isGrounded)
                --filter.CharacterController->JumpCount;

            --filter.CharacterController->DodgeCount;

            filter.CharacterController->GroundedDodge = isGrounded;
            filter.PhysicsBody->GravityScale = 0;

            if (filter.CharacterController->GroundedDodge)
                filter.CharacterController->DirectionValue.Y = 0;

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirX", filter.CharacterController->DirectionValue.X);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirY", filter.CharacterController->DirectionValue.Y);

            if (filter.CharacterController->DirectionValue.X != 0)
            {
                filter.CharacterController->MovementDirection = -FPMath.Sign(filter.CharacterController->DirectionValue.X).AsInt;
                f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, filter.CharacterController->MovementDirection);
            }
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, input, settings);

            MovementCurveSettings dodgeSettings = filter.CharacterController->GetDodgeSettings(settings);

            FPVector2 dodge = (filter.CharacterController->DirectionValue * dodgeSettings.Curve.Evaluate(filter.CharacterController->StateTime) * dodgeSettings.Force);
            filter.CharacterController->Velocity = dodge.X;
            filter.PhysicsBody->Velocity.Y = dodge.Y;
        }

        public override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            filter.PhysicsBody->GravityScale = 1;

            base.Exit(f, ref filter, input, settings);
        }
    }
}
