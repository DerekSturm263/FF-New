﻿using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class DodgeState : ActionState
    {
        [Header("State-Specific Values")]

        public AssetRefPlayerState Default;

        public MovementCurveSettingsXY SpotDodge;
        public MovementCurveSettingsXY ForwardRoll;
        public MovementCurveSettingsXY BackwardRoll;
        public MovementCurveSettingsXY AerialDodge;

        public MovementCurveSettings AerialGravity;
        public int MaxTimeToRoll;

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetDodgeSettings(this).Frames;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            bool canEnter = base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                filter.CharacterController->DodgeCount > 0 &&
                input.Movement.Magnitude >= settings.DeadStickZone;

            return canEnter;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            bool wasMoving = previousState == Default && filter.CharacterController->GroundedDodge;

            filter.CharacterController->GroundedDodge = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            if (filter.CharacterController->GroundedDodge)
            {
                --filter.CharacterController->JumpCount;

                filter.CharacterController->DirectionValue.X = FPMath.SignZeroInt(filter.CharacterController->DirectionValue.X);
                filter.CharacterController->DirectionValue.Y = 0;

                if (filter.CharacterController->DirectionValue.X == 0)
                    filter.CharacterController->DodgeType = DodgeType.Spot;
                else if (wasMoving)
                    filter.CharacterController->DodgeType = DodgeType.RollForward;
                else
                    filter.CharacterController->DodgeType = DodgeType.RollBackward;
            }
            else
            {
                filter.CharacterController->DodgeType = DodgeType.Aerial;
            }

            --filter.CharacterController->DodgeCount;

            if (!wasMoving && filter.CharacterController->DirectionValue.X != 0)
            {
                filter.CharacterController->SetDirection(f, -FPMath.SignInt(filter.CharacterController->DirectionValue.X), filter.Entity, filter.PlayerStats->Index);
            }

            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "RollForward", wasMoving);

            filter.CharacterController->Velocity = 0;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            MovementCurveSettingsXY dodgeSettings = filter.CharacterController->GetDodgeSettings(this);

            FP x = dodgeSettings.XCurve.Evaluate(filter.CharacterController->StateTime) * dodgeSettings.XForce;
            FP y = dodgeSettings.YCurve.Evaluate(filter.CharacterController->StateTime) * dodgeSettings.YForce;
            FPVector2 dodge = FPVector2.Scale(filter.CharacterController->DirectionValue, new(x, y));

            if (filter.CharacterController->DodgeType == DodgeType.Aerial)
            {
                dodge.Y += AerialGravity.Curve.Evaluate(filter.CharacterController->StateTime) * AerialGravity.Force;

                if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
                {
                    if (filter.CharacterController->StateTime < MaxTimeToRoll)
                        stateMachine.ForceTransition(f, ref filter, input, this, 3);
                    else
                        stateMachine.ForceTransition(f, ref filter, input, Default, 3);

                    filter.CharacterController->NextStateTime = filter.CharacterController->StateTime;
                }
            }

            filter.PhysicsBody->Velocity = dodge;
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.CharacterController->DodgeType = DodgeType.None;

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
