using Photon.Deterministic;
using Quantum.Custom.Animator;
using Quantum.Types;

namespace Quantum
{
    public unsafe sealed class UltimateState : PlayerState
    {
        public override States GetState() => States.IsUsingUltimate;

        public override bool GetInput(ref Input input) => input.Ultimate;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.MoveAnim.Id);

                if (animEvent.AnimID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animEvent.AnimID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate _) && filter.Stats->CurrentEnergy >= filter.Stats->MaxEnergy && filter.CharacterController->UltimateTime == 0;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.MoveAnim.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                ultimate.OnBegin(f, filter.Entity);
                filter.CharacterController->UltimateTime = ultimate.Length;
            }
        }
    }
}
