using System.Collections.Generic;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class JumpState : ActionState
    {
        public AssetRefPlayerState Interact;

        public MovementCurveSettings ShortJump;
        public MovementCurveSettings FullJump;
        public MovementCurveSettings AerialJump;

        public List<AssetRefPlayerState> StatesToResetVelocity;

        protected override int StateTime(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => filter.CharacterController->GetJumpSettings(this).Frames;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, stateMachine, ref filter, input, settings) &&
                filter.CharacterController->JumpCount > 0;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
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

            HandleJumping(f, stateMachine, ref filter, input, settings, stats);

            if (filter.CharacterController->WasReleasedThisFrame(input, Input.Buttons.SubWeapon) && filter.CharacterController->HasSubWeapon)
            {
                stateMachine.ForceTransition(f, ref filter, input, settings, Interact, settings.InputCheckTime);
            }
        }

        private void HandleJumping(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, ApparelStats stats)
        {
            MovementCurveSettings jumpSettings = filter.CharacterController->GetJumpSettings(this);
            filter.PhysicsBody->Velocity.Y = jumpSettings.Curve.Evaluate(filter.CharacterController->StateTime) * (jumpSettings.Force * (1 / stats.Weight));
        }

        public override void FinishExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            if (StatesToResetVelocity.Contains(nextState))
                filter.PhysicsBody->Velocity.Y /= 2;

            filter.CharacterController->JumpType = (JumpType)(-1);

            base.FinishExit(f, stateMachine, ref filter, input, settings, nextState);
        }
    }
}
