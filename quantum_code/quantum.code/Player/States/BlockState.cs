namespace Quantum
{
    public unsafe sealed class BlockState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsBlocking, StatesFlag.Block);

        public override bool GetInput(ref Input input) => input.Block;
        public override StateType GetStateType() => StateType.Grounded;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => -1;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting, States.IsDodging };

        protected override bool CanExit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return !input.Block || filter.CharacterController->IsInState(States.IsDodging);
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.CharacterController->Velocity = 0;
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 });
        }
    }
}
