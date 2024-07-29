using Quantum.Custom.Animator;

namespace Quantum
{
    public unsafe sealed class UltimateState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsUsingUltimate, 0);

        public override bool GetInput(ref Input input) => input.Ultimate;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            if (f.TryFindAsset(filter.PlayerStats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);

                if (animEvent.AnimID != 0)
                    return (CustomAnimator.GetStateFromId(f, filter.CustomAnimator, animEvent.AnimID).motion as AnimatorClip).data.frameCount;
            }

            return 1;
        }
        public override bool CanInterruptSelf => true;

        public override States[] EntranceBlacklist => new States[] { States.IsInteracting, States.IsBlocking, States.IsDodging, States.IsBursting };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !filter.CharacterController->IsCommitted && f.TryFindAsset(filter.PlayerStats->Build.Equipment.Ultimate.Id, out Ultimate _) && filter.Stats->CurrentStats.Energy >= f.Global->CurrentMatch.Ruleset.Players.MaxEnergy && filter.CharacterController->UltimateTime == 0;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            if (f.TryFindAsset(filter.PlayerStats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);
                CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                ultimate.OnBegin(f, filter.Entity);
                filter.CharacterController->UltimateTime = ultimate.Length;

                filter.CharacterController->PossibleStates = 0;
            }
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            filter.CharacterController->PossibleStates = (StatesFlag)511;
        }
    }
}
