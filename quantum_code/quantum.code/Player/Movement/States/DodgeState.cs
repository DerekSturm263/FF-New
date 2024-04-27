using Photon.Deterministic;
using Quantum.Types;

namespace Quantum.Movement
{
    public unsafe sealed class DodgeState : PlayerState
    {
        public override States GetState() => States.IsDodging;

        public override bool GetInput(ref Input input) => input.Block;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => filter.CharacterController->GetDodgeSettings(settings).Frames;
        protected override int DelayedEntranceTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => settings.DirectionChangeTime;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting };

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return input.Movement != default && filter.CharacterController->DodgeCount > 0;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.CharacterController->DodgeDirection = SnapTo8Directions(input.Movement);
            filter.PhysicsBody->GravityScale = 0;
        }

        protected override void DelayedEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.DelayedEnter(f, ref filter, ref input, settings);

            filter.CharacterController->DodgeDirection = SnapTo8Directions(input.Movement);

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirX", filter.CharacterController->DodgeDirection.X);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirY", filter.CharacterController->DodgeDirection.Y);

            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                filter.CharacterController->DodgeSettingsIndex = 0;
            }
            else
            {
                filter.CharacterController->DodgeSettingsIndex = 1;
                --filter.CharacterController->DodgeCount;
            }
        }

        public override void Update(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            if (filter.CharacterController->FramesInState > DelayedEntranceTime(f, ref filter, ref input, settings))
            {
                MovementCurveSettings dodgeSettings = filter.CharacterController->GetDodgeSettings(settings);
                filter.PhysicsBody->Velocity = (filter.CharacterController->DodgeDirection * dodgeSettings.Curve.Evaluate(filter.CharacterController->FramesInState) * dodgeSettings.Force);
            }
        }

        protected override void Exit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.PhysicsBody->GravityScale = 1;

            filter.CharacterController->DodgeSettingsIndex = 0;
        }

        private static FP DOT_SUCCESS = (FP)1 / 2;

        public static FPVector2 SnapTo8Directions(FPVector2 vector2)
        {
            FPVector2 dir = FPVector2.Zero;

            if (FPVector2.Dot(vector2, FPVector2.Up) > DOT_SUCCESS)
                dir += FPVector2.Up;
            else if (FPVector2.Dot(vector2, FPVector2.Down) > DOT_SUCCESS)
                dir += FPVector2.Down;

            if (FPVector2.Dot(vector2, FPVector2.Left) > DOT_SUCCESS)
                dir += FPVector2.Left;
            else if (FPVector2.Dot(vector2, FPVector2.Right) > DOT_SUCCESS)
                dir += FPVector2.Right;

            return dir.Normalized;
        }
    }
}
