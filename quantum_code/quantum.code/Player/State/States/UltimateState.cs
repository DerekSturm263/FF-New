using Photon.Deterministic;
using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class UltimateState : PlayerState
    {
        public override States GetState() => States.IsUsingUltimate;

        public override bool GetInput(ref Input input) => input.Ultimate;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                if (ultimate.Move.ID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, ultimate.Move.ID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate _) && filter.Stats->CurrentEnergy >= filter.Stats->MaxEnergy && filter.CharacterController->UltimateTime == 0;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, ultimate.Move.ID);

                ultimate.OnBegin(f, filter.Entity);
                filter.CharacterController->UltimateTime = ultimate.Length;
            }

            StatsSystem.SetEnergy(f, filter.PlayerLink, filter.Stats, 0);
        }
    }
}
