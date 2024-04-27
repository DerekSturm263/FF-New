using System.Collections.Generic;

namespace Quantum.Movement
{
    public unsafe class PlayerStateSystem : SystemMainThreadFilter<PlayerStateSystem.Filter>
    {
        public static Dictionary<States, PlayerState> AllStates = new() {
            [States.IsCrouching] = new CrouchState(),
            [States.IsJumping] = new JumpState(),
            [States.IsFastFalling] = new FastFallState(),
            [States.IsBursting] = new BurstState(),
            [States.IsDodging] = new DodgeState(),
            [States.IsBlocking] = new BlockState(),
            [States.IsEmoting] = new EmoteState(),
            [States.IsUsingUltimate] = new UltimateState(),
            [States.IsInteracting] = new InteractState(),
            [States.IsUsingMainWeapon] = new MainWeaponState(),
            [States.IsUsingSubWeapon] = new SubWeaponState(),
            [States.IsUsingSkill] = new SkillState(),
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

            // Get all the nearby colliders.
            filter.CharacterController->NearbyColliders = filter.CharacterController->GetNearbyColliders(f, settings, filter.Transform);

            // Get if the player is grounded or not...
            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = 2;
                filter.CharacterController->DodgeCount = 1;
            }

            // Update any miscellaneous CustomAnimator values.
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsGrounded", filter.CharacterController->GetNearbyCollider(Colliders.Ground));
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", filter.PhysicsBody->Velocity.Y);

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

                // Set whether or not a given button is being held to prevent states from triggering multiple times.
                filter.CharacterController->SetIsHolding(state, AllStates[state].GetInput(ref input));
            }

            // Handle the left/right movement.
            HandleMovement(f, ref filter, ref input, settings);

            // Increment the state number.
            ++filter.CharacterController->FramesInState;
        }

        private void HandleMovement(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings)
        {
            if (!filter.CharacterController->IsInState(States.IsCrouching))
            {
                filter.CharacterController->Move(f, input.Movement.X, filter.Transform, filter.PhysicsBody, filter.CustomAnimator, movementSettings, f.DeltaTime);
            }
        }
    }
}
