using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class BlockState : InputState
    {
        public FPVector2 KnockbackMultiplier;

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);
        
            filter.CharacterController->KnockbackMultiplier = KnockbackMultiplier;
        }

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.CharacterController->KnockbackMultiplier = FPVector2.One;
            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)((int)HurtboxType.Head * 2 - 1), HurtboxSettings.Default, false);

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
