using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class UltimateState : PlayerState
    {
        public override States GetState() => States.IsUsingUltimate;

        public override bool GetInput(ref Input input) => input.Ultimate;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => -1;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.Stats->CurrentEnergy >= filter.Stats->MaxEnergy;
        }

        protected override bool CanExit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CustomAnimator->normalized_time == 1;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            if (f.TryFindAsset(filter.Stats->Build.Equipment.WeaponSettings.Ultimate.Id, out Ultimate ultimate))
            {
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, ultimate.Move.ID);
            }
        }
    }
}
