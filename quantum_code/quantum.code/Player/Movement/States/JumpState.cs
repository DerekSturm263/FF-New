namespace Quantum.Movement
{
    public unsafe sealed class JumpState : MovementState
    {
        public override States GetState() => States.IsJumping;

        public override bool GetInput(ref Input input) => input.Jump;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;

        protected override bool CanEnter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return !filter.CharacterController->IsHoldingJump && filter.CharacterController->JumpCount > 0;
        }

        protected override bool CanExit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->JumpTime >= filter.CharacterController->GetJumpSettings(settings).Frames;
        }

        protected override void Enter(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            if (!filter.CharacterController->IsGrounded)
            {
                filter.CharacterController->JumpSettingsIndex = 2;
            }
            if (filter.CharacterController->IsAgainstWall != 0)
            {
                // 6 -> Left Wall Jump
                // 8 -> Right Wall Jump
                filter.CharacterController->JumpSettingsIndex = 7 + filter.CharacterController->IsAgainstWall;
            }

            --filter.CharacterController->JumpCount;
            filter.CharacterController->IsHoldingJump = true;
        }

        public override void Update(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            ++filter.CharacterController->JumpTime;

            if (filter.CharacterController->JumpTime < settings.FullHopFrameMin && filter.CharacterController->JumpSettingsIndex < 2)
            {
                if (!input.Jump)
                {
                    filter.CharacterController->JumpSettingsIndex = 0;
                    filter.CharacterController->JumpTime = settings.FullHopFrameMin;

                    CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "JumpStrength", (Photon.Deterministic.FP)1 / 2);
                }
                else
                {
                    filter.CharacterController->JumpSettingsIndex = 1;

                    CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "JumpStrength", 1);
                }
            }
            else
            {
                MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(settings);
                filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->JumpTime) * jumpSettings.Force;
            }
        }

        protected override void Exit(Frame f, ref MovementSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.CharacterController->JumpSettingsIndex = 0;
            filter.CharacterController->JumpTime = 0;
        }
    }
}
