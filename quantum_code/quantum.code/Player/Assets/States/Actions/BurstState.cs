using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class BurstState : ActionState
    {
        public FP EnergyCost;
        public int Time;

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => Time;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                filter.Stats->CurrentStats.Energy >= EnergyCost;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PhysicsBody->Velocity = FPVector2.Zero;

            filter.CharacterController->CanMove = true;
            filter.CharacterController->MaintainVelocity = false;

            StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -EnergyCost);
        }
    }
}
