using Photon.Deterministic;

namespace Quantum.Movement
{
    public unsafe sealed class DodgeState : MovementState
    {
        public override States GetState() => States.IsDodging;

        public override bool GetInput(ref Input input) => input.Block;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;

        public override States[] EntranceBlacklist => new States[] { States.IsBursting };

        protected override bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return !filter.CharacterController->IsHoldingDodge && input.Movement != default && filter.CharacterController->DodgeCount > 0;
        }

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->DodgeTime >= filter.CharacterController->GetDodgeSettings(settings, filter.CharacterController->IsGrounded).Frames;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            filter.CharacterController->DodgeDirection = SnapTo8Directions(input.Movement);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirX", filter.CharacterController->DodgeDirection.X);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirY", filter.CharacterController->DodgeDirection.Y);

            if (filter.CharacterController->IsGrounded)
            {
                filter.CharacterController->DodgeSettingsIndex = 0;
            }
            else
            {
                filter.CharacterController->DodgeSettingsIndex = 1;
                --filter.CharacterController->DodgeCount;
            }

            filter.PhysicsBody->GravityScale = 0;
            filter.CharacterController->IsHoldingDodge = true;
        }

        public override void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            ++filter.CharacterController->DodgeTime;

            if (filter.CharacterController->DodgeTime < settings.DodgeFrameMin)
            {
                if (filter.CharacterController->DodgeDirection == default)
                {
                    filter.CharacterController->DodgeDirection = SnapTo8Directions(input.Movement);

                    CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirX", filter.CharacterController->DodgeDirection.X);
                    CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "DodgeDirY", filter.CharacterController->DodgeDirection.Y);
                }
            }
            else
            {
                MovementCurveSettings dodgeSettings = filter.CharacterController->GetDodgeSettings(settings, filter.CharacterController->IsGrounded);
                filter.PhysicsBody->Velocity = (filter.CharacterController->DodgeDirection * dodgeSettings.Curve.Evaluate(filter.CharacterController->DodgeTime) * dodgeSettings.Force);
            }
        }

        protected override void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.PhysicsBody->GravityScale = 1;

            filter.CharacterController->DodgeSettingsIndex = 0;
            filter.CharacterController->DodgeTime = 0;
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
