using System.Collections.Generic;

namespace Quantum.Movement
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
    {
        public static Dictionary<States, MovementState> AllStates = new() {
            [States.IsCrouching] = new CrouchState(),
            [States.IsJumping] = new JumpState(),
            [States.IsFastFalling] = new FastFallState(),
            [States.IsBursting] = new BurstState(),
            [States.IsBlocking] = new BlockState(),
            [States.IsDodging] = new DodgeState(),
            [States.IsEmoting] = new EmoteState(),
            [States.IsAttacking] = new AttackState(),
            [States.IsInteracting] = new InteractState(),
        };

        public struct Filter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public CustomAnimator* CustomAnimator;
            public PlayerLink* PlayerLink;
            public Stats* Stats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Grab the player's movement settings.
            MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->MovementSettings.Id);

            // Get if the player is grounded or not...
            filter.CharacterController->IsGrounded = filter.CharacterController->GetIsGrounded(f, settings, filter.Transform) && !filter.CharacterController->IsInState(States.IsJumping);
            if (filter.CharacterController->IsGrounded)
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = 2;
                filter.CharacterController->DodgeCount = 1;
            }

            // Update any miscellaneous CustomAnimator values.
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsGrounded", filter.CharacterController->IsGrounded);
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", filter.PhysicsBody->Velocity.Y);

            // Get if the player is against a wall or not (and if so, which direction its in).
            filter.CharacterController->IsAgainstWall = filter.CharacterController->GetIsAgainstWall(f, settings, filter.Transform);

            // Get the player's input before we do anything with it.
            Input input = *f.GetPlayerInput(filter.PlayerLink->Player);

            // Iterate through all possible states the player can be in.
            foreach (States state in AllStates.Keys)
            {
                // Check if they're in a given state...
                if (filter.CharacterController->IsInState(state))
                {
                    // If they are, try to see if they should leave the state...
                    if (!AllStates[state].TryExitAndResolveState(f, ref filter, ref input, settings))
                    {
                        // If they shouldn't, update the state.
                        AllStates[state].Update(f, ref filter, ref input, settings);
                    }
                }
                else
                {
                    // If they aren't, try to see if the should enter the state...
                    AllStates[state].TryEnterAndResolveState(f, ref filter, ref input, settings);
                }
            }

            // TEMP: Delete since I don't like this.
            if (!input.Jump)
                filter.CharacterController->IsHoldingJump = false;

            if (!input.Block || input.Movement == default)
                filter.CharacterController->IsHoldingDodge = false;

            // Handle the left-right movement.
            HandleMovement(f, ref filter, ref input, settings);
        }

        private void HandleMovement(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings)
        {
            if (!filter.CharacterController->IsInState(States.IsCrouching))
            {
                filter.CharacterController->Move(f, input.Movement.X, filter.Transform, filter.PhysicsBody, filter.CustomAnimator, movementSettings, filter.CharacterController->IsGrounded, f.DeltaTime);
            }
        }
    }
}
