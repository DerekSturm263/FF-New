using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe class CharacterControllerSystem : SystemMainThreadFilter<CharacterControllerSystem.Filter>, ISignalOnMapChanged
    {
        public static PlayerStateMachine AllStates =
        new(
            [
                new DefaultState(),
                new CrouchState(),
                new LookUpState(),
                new JumpState(),
                new DodgeState(),
                new BlockState(),
                new BurstState(),
                new EmoteState(),
                new SubState(),
                new UltimateState(),
                new InteractState(),
                new PrimaryWeaponState(),
                new SecondaryWeaponState()
            ]
        );

        public struct Filter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public CustomAnimator* CustomAnimator;
            public PlayerStats* PlayerStats;
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

            Log.Debug(input.InputButtons);

            // Resolve the State Machine to determine which state the player should be in.
            AllStates.Resolve(f, ref filter, input, settings);

            // Apply knockback velocity.
            if (filter.CharacterController->KnockbackVelocityTime > 0)
            {
                filter.CharacterController->KnockbackVelocityTime -= f.DeltaTime;
                filter.CharacterController->KnockbackVelocityX -= f.DeltaTime;
                filter.CharacterController->Influence += f.DeltaTime;

                if (filter.CharacterController->KnockbackVelocityTime <= 0)
                {
                    filter.CharacterController->KnockbackVelocityX = 0;
                    filter.CharacterController->Influence = 1;
                }
            }

            // Do everything.
            HandleGround(f, filter, settings);
            HandleUltimate(f, filter);
        }

        public void OnMapChanged(Frame f, AssetRefMap previousMap)
        {
            var playerFilter = f.Unsafe.FilterStruct<Filter>();
            var player = default(Filter);

            while (playerFilter.Next(&player))
            {
                player.Transform->Position = ArrayHelper.All(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints)[player.PlayerStats->Index.Global];
            }
        }

        private void HandleGround(Frame f, Filter filter, MovementSettings movementSettings)
        {
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            // Get all the nearby colliders.
            filter.CharacterController->NearbyColliders = filter.CharacterController->GetNearbyColliders(f, movementSettings, filter.Transform);
            if (filter.CharacterController->CurrentState == States.Default && filter.PhysicsBody->Velocity.Y > 0)
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

        private void HandleUltimate(Frame f, Filter filter)
        {
            // Decrease the time left in the Ultimate state if the player is using their Ultimate.
            if (filter.CharacterController->UltimateTime > 0)
            {
                if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate ultimate))
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
