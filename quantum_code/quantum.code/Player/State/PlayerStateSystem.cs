using System.Collections.Generic;

namespace Quantum.Movement
{
    public unsafe class PlayerStateSystem : SystemMainThreadFilter<PlayerStateSystem.Filter>, ISignalOnComponentAdded<CharacterController>
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
            [States.IsUsingSubWeapon] = new SubWeaponState()
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
            if (filter.CharacterController->IsInState(States.IsJumping))
                filter.CharacterController->NearbyColliders &= ~Colliders.Ground;

            // Get if the player is grounded or not...
            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = filter.CharacterController->MaxJumpCount;
                filter.CharacterController->DodgeCount = filter.CharacterController->MaxDodgeCount;
            }

            // Update any miscellaneous CustomAnimator values.
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsGrounded", filter.CharacterController->GetNearbyCollider(Colliders.Ground));
            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", filter.PhysicsBody->Velocity.Y);

            // Get the player's input before we do anything with it.
            Input input = *f.GetPlayerInput(filter.PlayerLink->Player);

            // Grab the player's stats from their outfit.
            ApparelStats stats = ApparelHelper.Default;
            {
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Headgear), stats);
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Clothing), stats);
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Legwear), stats);
            }

            // Multiply apparel stats by multipler.
            stats = ApparelHelper.Multiply(filter.Stats->ApparelStatsMultiplier, stats);

            // Iterate through all possible states the player can be in.
            foreach (States state in AllStates.Keys)
            {
                // Check if they're in a given state...
                if (filter.CharacterController->IsInState(state))
                {
                    // If they are, try to see if they should leave the state...
                    if (!AllStates[state].TryExitAndResolveState(f, ref filter, ref input, settings, stats))
                    {
                        // Some states can be interrupted by themselves, this lets that happen.
                        if (!AllStates[state].CanInterruptSelf || !AllStates[state].TryEnterAndResolveState(f, ref filter, ref input, settings, stats))
                        {
                            // If they shouldn't, update the state.
                            AllStates[state].Update(f, ref filter, ref input, settings, stats);
                        }
                    }
                }
                else
                {
                    // If they aren't, try to see if the should enter the state...
                    AllStates[state].TryEnterAndResolveState(f, ref filter, ref input, settings, stats);
                }

                // Set whether or not a given button is being held to prevent states from triggering multiple times.
                filter.CharacterController->SetIsHolding(state, AllStates[state].GetInput(ref input));
            }

            // Handle the left/right movement.
            HandleMovement(f, ref filter, ref input, settings, stats);

            // Increment the state number.
            ++filter.CharacterController->StateTime;

            // Decrease the time left in the Ultimate state if the player is using their Ultimate.
            if (filter.CharacterController->UltimateTime > 0)
            {
                filter.CharacterController->UltimateTime--;
                if (filter.CharacterController->UltimateTime == 0)
                {
                    // Invoke the Ultimate's End function once time runs out to reset stats and stuff.
                    if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
                    {
                        ultimate.OnEnd(f, filter.Entity);
                    }
                }
            }
        }

        public void OnAdded(Frame f, EntityRef entity, CharacterController* component)
        {
            MovementSettings movementSettings = f.FindAsset<MovementSettings>(component->MovementSettings.Id);

            component->MaxJumpCount = movementSettings.MaxJumpCount;
            component->MaxDodgeCount = movementSettings.MaxDodgeCount;
        }

        private void HandleMovement(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings, ApparelStats stats)
        {
            if (!filter.CharacterController->CanInput)
                return;

            if (!filter.CharacterController->IsInState(States.IsCrouching))
            {
                filter.CharacterController->Move(f, input.Movement.X, filter.Transform, filter.PhysicsBody, filter.CustomAnimator, movementSettings, stats);
            }
        }
    }
}
