using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe class CharacterControllerSystem : SystemMainThreadFilter<CharacterControllerSystem.Filter>, ISignalOnMapChanged
    {
        public static PlayerStateDictionary AllStates =
        new(
            new CrouchState(),
            new JumpState(),
            new FastFallState(),
            new BurstState(),
            new DodgeState(),
            new BlockState(),
            new EmoteState(),
            new SubState(),
            new UltimateState(),
            new InteractState()
            //new MainWeaponState(),
            //new AltWeaponState()
        );

        public struct Filter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public CustomAnimator* CustomAnimator;
            public Stats* Stats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Grab the player's movement settings.
            MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->Settings.Id);

            // Get the entity's input before we do anything with it.
            Input input;
            if (f.Unsafe.TryGetPointer(filter.Entity, out AIData* aiData) &&
                f.TryFindAsset(aiData->Behavior.Id, out Behavior behavior))
                input = behavior.GetInput(f, filter);
            else
                input = *f.GetPlayerInput(f.Get<PlayerLink>(filter.Entity).Player);

            // Calculate the player's stats.
            ApparelStats stats = CalculateStats(f, ref filter, settings);

            // Do everything.
            HandleGround(f, ref filter, settings, stats);
            HandleStateSwitching(f, ref filter, ref input, settings, stats);
            HandleMovement(f, ref filter, ref input, settings, stats);
            HandleUltimate(f, ref filter, ref input, settings, stats);
        }

        public void OnMapChanged(Frame f, AssetRefMap previousMap)
        {
            var playerFilter = f.Unsafe.FilterStruct<Filter>();
            var player = default(Filter);

            while (playerFilter.Next(&player))
            {
                player.Transform->Position = ArrayHelper.Get(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints, player.Stats->GlobalIndex);
            }
        }

        private ApparelStats CalculateStats(Frame f, ref Filter filter, MovementSettings movementSettings)
        {
            // Grab the player's stats from their outfit.
            ApparelStats stats = ApparelHelper.Default;
            {
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Headgear), stats);
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Clothing), stats);
                stats = ApparelHelper.Add(ApparelHelper.FromApparel(f, filter.Stats->Build.Equipment.Outfit.Legwear), stats);
            }

            // Multiply apparel stats by multipler.
            stats = ApparelHelper.Multiply(filter.Stats->ApparelStatsMultiplier, stats);
            return stats;
        }

        private void HandleGround(Frame f, ref Filter filter, MovementSettings movementSettings, ApparelStats stats)
        {
            // Get all the nearby colliders.
            filter.CharacterController->NearbyColliders = filter.CharacterController->GetNearbyColliders(f, movementSettings, filter.Transform);
            if (filter.CharacterController->IsInState(States.IsJumping))
                filter.CharacterController->NearbyColliders &= ~Colliders.Ground;

            // Get if the player is grounded or not...
            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = stats.Jump.AsShort;
                filter.CharacterController->DodgeCount = stats.Dodge.AsShort;
            }

            // Update any miscellaneous CustomAnimator values.
            bool isGrounded = filter.CharacterController->GetNearbyCollider(Colliders.Ground);
            CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsGrounded", isGrounded);

            if (isGrounded)
                CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", 0);
            else
                CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "YVelocity", filter.PhysicsBody->Velocity.Y);
        }

        private void HandleStateSwitching(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings, ApparelStats stats)
        {
            // Iterate through all possible states the player can be in.
            foreach (States state in AllStates.Keys)
            {
                // Check if they're in a given state...
                if (filter.CharacterController->IsInState(state))
                {
                    // If they are, try to see if they should leave the state...
                    if (!AllStates[state].TryExitAndResolveState(f, ref filter, ref input, movementSettings, stats))
                    {
                        // Some states can be interrupted by themselves, this lets that happen.
                        if (!AllStates[state].CanInterruptSelf || !AllStates[state].TryEnterAndResolveState(f, ref filter, ref input, movementSettings, stats))
                        {
                            // If they shouldn't, update the state.
                            AllStates[state].Update(f, ref filter, ref input, movementSettings, stats);
                        }
                    }
                }
                else
                {
                    // If they aren't, try to see if the should enter the state...
                    AllStates[state].TryEnterAndResolveState(f, ref filter, ref input, movementSettings, stats);
                }

                // Set whether or not a given button is being held to prevent states from triggering multiple times.
                filter.CharacterController->SetIsHolding(state, AllStates[state].GetInput(ref input));
            }
        }

        private void HandleMovement(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings, ApparelStats stats)
        {
            // Make sure the user can input.
            if (!filter.CharacterController->CanInput)
                return;

            // Check to see if the user is in any non-moving state, and if they aren't, let them move.
            if (!filter.CharacterController->IsInState(States.IsCrouching, States.IsInteracting, States.IsBlocking, States.IsBursting, States.IsDodging))
                filter.CharacterController->Move(f, input.Movement.X, ref filter, movementSettings, stats);

            // Apply velocity to the player.
            filter.PhysicsBody->Velocity.X = filter.CharacterController->Velocity * stats.Agility;
        }

        private void HandleUltimate(Frame f, ref Filter filter, ref Input input, MovementSettings movementSettings, ApparelStats stats)
        {
            // Increment the state number.
            ++filter.CharacterController->StateTime;

            // Decrease the time left in the Ultimate state if the player is using their Ultimate.
            if (filter.CharacterController->UltimateTime > 0)
            {
                if (f.TryFindAsset(filter.Stats->Build.Equipment.Ultimate.Id, out Ultimate ultimate))
                {
                    filter.CharacterController->UltimateTime--;

                    if (filter.CharacterController->UltimateTime == 0)
                    {
                        // Invoke the Ultimate's End function once time runs out to reset stats and stuff.
                        ultimate.OnEnd(f, filter.Entity);
                    }
                }

                // Set the user's energy to show how much time they have left.
                StatsSystem.SetEnergy(f, filter.Entity, filter.Stats, ((FP)filter.CharacterController->UltimateTime / ultimate.Length) * f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
            }
        }
    }
}
