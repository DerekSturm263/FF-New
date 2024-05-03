using Photon.Deterministic;

namespace Quantum.Movement
{
    public unsafe sealed class BurstState : PlayerState
    {
        public override States GetState() => States.IsBursting;

        public override bool GetInput(ref Input input) => input.Burst;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats) => settings.BurstTime;

        public override States[] KillStateList => new States[] { States.IsBlocking, States.IsDodging };

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            return filter.Stats->CurrentEnergy >= settings.BurstCost;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Enter(f, ref filter, ref input, settings, stats);

            filter.PhysicsBody->GravityScale = 0;
            StatsSystem.ModifyEnergy(f, filter.PlayerLink, filter.Stats, -settings.BurstCost);
        }

        protected override void Exit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings, ApparelStats stats)
        {
            base.Exit(f, ref filter, ref input, settings, stats);

            filter.PhysicsBody->GravityScale = 1;
        }
    }
}
