using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class DodgeState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Dodge;

        public override (States, StatesFlag) GetStateInfo() => (States.Dodge, StatesFlag.Dodge);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override bool OverrideDirection() => true;

        public override TransitionInfo[] GetTransitions(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Burst, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetDodgeSettings(settings).Frames;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            bool canEnter = base.CanEnter(f, ref filter, input, settings) &&
                filter.CharacterController->DodgeCount > 0 &&
                input.Movement.Magnitude >= settings.DeadStickZone;

            Log.Debug($"Can Enter Dodge: {canEnter}");

            return canEnter;
        }

        public override void FinishEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, ref filter, input, settings, previousState);

            bool wasMoving = previousState == States.Default && filter.CharacterController->GroundedDodge;
            filter.PhysicsBody->GravityScale = 0;

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
                filter.CharacterController->MovementDirection = -FPMath.SignInt(filter.CharacterController->DirectionValue.X);
                f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, filter.CharacterController->MovementDirection);
            }

            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "Roll Forward", wasMoving);

            filter.CharacterController->Velocity = 0;
        }

        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, ref filter, input, settings);

            MovementCurveSettingsXY dodgeSettings = filter.CharacterController->GetDodgeSettings(settings);

            FP x = dodgeSettings.XCurve.Evaluate(filter.CharacterController->StateTime) * dodgeSettings.XForce;
            FP y = dodgeSettings.YCurve.Evaluate(filter.CharacterController->StateTime) * dodgeSettings.YForce;
            FPVector2 dodge = FPVector2.Scale(filter.CharacterController->DirectionValue, new(x, y));

            if (filter.CharacterController->DodgeType == DodgeType.Aerial)
                dodge.Y += settings.AerialGravity.Curve.Evaluate(filter.CharacterController->StateTime) * settings.AerialGravity.Force;

            filter.PhysicsBody->Velocity = dodge;
        }

        public override void FinishExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            filter.PhysicsBody->GravityScale = 1;
            filter.CharacterController->DodgeType = (DodgeType)(-1);

            base.FinishExit(f, ref filter, input, settings, nextState);
        }
    }
}
