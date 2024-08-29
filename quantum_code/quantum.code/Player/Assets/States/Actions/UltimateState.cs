namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class UltimateState : ActionState
    {
        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);

                if (animEvent is not null && animEvent.AnimID != 0)
                    return CustomAnimator.GetStateLength(f, filter.CustomAnimator, animEvent.AnimID);
            }

            return 0;
        }

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate _) &&
                filter.Stats->CurrentStats.Energy >= f.Global->CurrentMatch.Ruleset.Players.MaxEnergy &&
                filter.CharacterController->UltimateTime == 0;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);
                if (animEvent is not null)
                    CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                ultimate.OnBegin(f, filter.Entity);
                
                filter.CharacterController->UltimateTime = ultimate.Length;
                ++filter.PlayerStats->Stats.UltimateUses;
            }
        }
    }
}