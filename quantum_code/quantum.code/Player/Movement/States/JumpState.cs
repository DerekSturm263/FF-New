﻿using System.Runtime;

namespace Quantum.Movement
{
    public unsafe sealed class JumpState : PlayerState
    {
        public override States GetState() => States.IsJumping;

        public override bool GetInput(ref Input input) => input.Jump;
        public override StateType GetStateType() => StateType.Grounded | StateType.Aerial;
        protected override int StateTime(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings) => filter.CharacterController->GetJumpSettings(settings).Frames;

        protected override bool CanEnter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            return filter.CharacterController->JumpCount > 0;
        }

        protected override void Enter(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, ref input, settings);

            if (!filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                filter.CharacterController->JumpSettingsIndex = 2;
            }
            if (filter.CharacterController->GetNearbyCollider(Colliders.LeftWall | Colliders.RightWall))
            {
                // 6 -> Left Wall Jump
                // 8 -> Right Wall Jump
                filter.CharacterController->JumpSettingsIndex = 7;
            }

            --filter.CharacterController->JumpCount;
        }

        public override void Update(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, ref input, settings);

            if (filter.CharacterController->FramesInState < settings.FullHopFrameMin && filter.CharacterController->JumpSettingsIndex < 2)
            {
                if (!input.Jump)
                {
                    filter.CharacterController->JumpSettingsIndex = 0;
                    filter.CharacterController->FramesInState = settings.FullHopFrameMin; // TODO: CHANGE

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
                filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->FramesInState) * jumpSettings.Force;
            }
        }

        protected override void Exit(Frame f, ref PlayerStateSystem.Filter filter, ref Input input, MovementSettings settings)
        {
            base.Exit(f, ref filter, ref input, settings);

            filter.CharacterController->JumpSettingsIndex = 0;
        }
    }
}
