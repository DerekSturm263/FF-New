using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class CrouchState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsCrouching, StatesFlag.Crouch);

        public override bool GetInput(ref Input input) => input.Crouch;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => -1;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return FPMath.Abs(input.Movement.X) < FP._0_01;
        }

        protected override bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !input.Crouch;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Velocity = 0;
        }
    }
}
