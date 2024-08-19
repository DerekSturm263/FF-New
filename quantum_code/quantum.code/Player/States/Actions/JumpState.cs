﻿namespace Quantum
{
    public unsafe sealed class JumpState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Jump;

        public override (States, StatesFlag) GetStateInfo() => (States.Jump, StatesFlag.Jump);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Burst, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Dodge, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: true, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: true, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Block, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetJumpSettings(settings).Frames;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                filter.CharacterController->JumpCount > 0;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            filter.CharacterController->GroundedJump = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            if (filter.CharacterController->GroundedJump)
            {
                filter.CharacterController->NearbyColliders &= ~Colliders.Ground;

                if (!filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Jump) || filter.CharacterController->IsHeldThisFrame(input, Input.Buttons.Crouch))
                    filter.CharacterController->JumpType = JumpType.ShortHop;
                else
                    filter.CharacterController->JumpType = JumpType.FullHop;
            }
            else
            {
                filter.CharacterController->JumpType = JumpType.Aerial;
            }

            --filter.CharacterController->JumpCount;

            f.Events.OnPlayerJump(filter.Entity, filter.PlayerStats->Index, stats.Jump.AsInt - filter.CharacterController->JumpCount);
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            // Calculate the player's stats.
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            HandleMovement(f, stateMachine, ref filter, input, settings, stats);
            HandleJumping(f, stateMachine, ref filter, input, settings, stats);
            HandleFastFalling(f, stateMachine, ref filter, input, settings, stats);

            if (filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.SubWeapon) && filter.CharacterController->HasSubWeapon)
            {
                stateMachine.BeginTransition(f, ref filter, input, settings, new(States.Interact, settings.InputCheckTime, true, true));
            }
        }

        private void HandleMovement(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            filter.CharacterController->Move(f, input.Movement.X, ref filter, settings, stats);
        }

        private void HandleJumping(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(settings);
            filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->StateTime) * (jumpSettings.Force * (1 / stats.Weight));
        }

        private void HandleFastFalling(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            if (filter.CharacterController->WasPressedThisFrame(input, Input.Buttons.Crouch) && filter.PhysicsBody->Velocity.Y < settings.MinimumYVelocity)
            {
                filter.PhysicsBody->Velocity.Y = settings.FastFallForce;
            }
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            if (nextState == States.Interact || nextState == States.Sub || nextState == States.Primary || nextState == States.Secondary || nextState == States.Ultimate)
                filter.PhysicsBody->Velocity.Y = 0;

            filter.CharacterController->JumpType = (JumpType)(-1);

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}