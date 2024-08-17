using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class DefaultState : PassiveState
    {
        protected override bool IsInputting(ref CharacterControllerSystem.Filter filter, ref Input input) => true;

        public override (States, StatesFlag) GetStateInfo() => (States.Default, StatesFlag.Move | StatesFlag.Jump | StatesFlag.FastFall);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new(false, (f, filter, input, settings) => !filter.CharacterController->GetNearbyCollider(Colliders.Ground)) { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Block },
            new(false, (f, filter, input, settings) => FPMath.Abs(input.Movement.X) < settings.DeadStickZone) { Destination = States.Crouch },
            new(false, (f, filter, input, settings) => FPMath.Abs(input.Movement.X) < settings.DeadStickZone) { Destination = States.LookUp },
        ];

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, input, settings);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, ref filter, input, settings, stats);
            HandleFastFalling(f, ref filter, input, settings, stats);
        }

        private void HandleMovement(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, stats);
        }

        private void HandleFastFalling(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
            }
        }

        protected override bool DoExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => true;
    }
}
