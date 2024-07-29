using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class BurstState : PlayerState
    {
        public override (States, StatesFlag) GetState() => (States.IsBursting, StatesFlag.Burst);

        public override bool GetInput(ref Input input) => input.Burst;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.BurstTime;

        public override States[] KillStateList => new States[] { States.IsUsingSubWeapon };

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.Stats->CurrentStats.Energy >= settings.BurstCost;
        }

        protected override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.PhysicsBody->GravityScale = 0;
            filter.PhysicsBody->Velocity = FPVector2.Zero;
            filter.CharacterController->Velocity = 0;

            StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -settings.BurstCost);
        }

        protected override void Exit(Frame f, ref CharacterControllerSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 });
            filter.PhysicsBody->GravityScale = 1;
        }
    }
}
