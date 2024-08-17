using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class BurstState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Burst;

        public override (States, StatesFlag) GetStateInfo() => (States.Burst, StatesFlag.Burst);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Ultimate },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => settings.BurstTime;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                filter.Stats->CurrentStats.Energy >= settings.BurstCost;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            filter.PhysicsBody->GravityScale = 0;
            filter.PhysicsBody->Velocity = FPVector2.Zero;
            filter.CharacterController->Velocity = 0;

            StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -settings.BurstCost);
        }

        public override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            filter.PhysicsBody->GravityScale = 1;

            base.Exit(f, ref filter, input, settings);
        }
    }
}
