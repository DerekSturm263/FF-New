namespace Quantum
{
    public unsafe sealed class JumpState : DirectionalActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Jump;

        public override (States, StatesFlag) GetStateInfo() => (States.Jump, StatesFlag.Jump);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new(true) { Destination = States.Dodge },
            new(true) { Destination = States.Interact },
            new(true) { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new(true) { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetJumpSettings(settings).Frames;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                filter.CharacterController->JumpCount > 0;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            filter.CharacterController->GroundedJump = filter.CharacterController->GetNearbyCollider(Colliders.Ground);

            --filter.CharacterController->JumpCount;

            if (filter.CharacterController->GroundedJump && (!filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Jump)) || filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Crouch))
                filter.CharacterController->JumpSettingsIndex = 0;
            else
                filter.CharacterController->JumpSettingsIndex = 1;

            f.Events.OnPlayerJump(filter.Entity, filter.PlayerStats->Index, stats.Jump.AsInt - filter.CharacterController->JumpCount);
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, input, settings);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, ref filter, input, settings, stats);
            HandleJumping(f, ref filter, input, settings, stats);
            HandleFastFalling(f, ref filter, input, settings, stats);
        }

        private void HandleMovement(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, stats);
        }

        private void HandleJumping(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(settings);
            filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->StateTime) * (jumpSettings.Force * (1 / stats.Weight));
        }

        private void HandleFastFalling(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
            }
        }

        public override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, input, settings);

            filter.CharacterController->JumpSettingsIndex = 0;
        }
    }
}
