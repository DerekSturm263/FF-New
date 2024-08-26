using Photon.Deterministic;
using Quantum.Types;
using System;

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
            public Shakeable* Shakeable;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Grab the player's movement settings.
            MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->Settings.Id);

            // Get the entity's input before we do anything with it.
            Input input;
            if (f.Unsafe.TryGetPointer(filter.Entity, out AIData* aiData) && f.TryFindAsset(aiData->Behavior.Id, out Behavior behavior))
                input = behavior.GetInput(f, filter);
            else
                input = *f.GetPlayerInput(f.Get<PlayerLink>(filter.Entity).Player);

            // Handle some miscellaneous logic.
            HandleGround(f, filter, settings);
            HandleUltimate(f, filter);

            // Resolve the State Machine to determine which state the player should be in.
            AllStates.Resolve(f, ref filter, input, settings);

            // Set the user's knockback velocity once they stop shaking.
            if (filter.Shakeable->Time <= 0 && !filter.CharacterController->DeferredKnockback.Equals(default(KnockbackInfo)))
            {
                filter.CharacterController->CurrentKnockback = filter.CharacterController->DeferredKnockback;
                filter.CharacterController->DeferredKnockback = default;

                filter.PhysicsBody->Velocity.Y = filter.CharacterController->CurrentKnockback.Direction.Y;
            }

            // Apply the knockback velocity.
            if (filter.CharacterController->CurrentKnockback.Time > 0)
            {
                filter.CharacterController->CurrentKnockback.Time -= f.DeltaTime;
                filter.CharacterController->CurrentKnockback.Direction.X -= f.DeltaTime;

                filter.CharacterController->Influence += f.DeltaTime;

                if (filter.CharacterController->CurrentKnockback.Time <= 0)
                {
                    filter.CharacterController->CurrentKnockback.Direction.X = 0;
                    filter.CharacterController->Influence = 1;
                }

                PreviewKnockback(filter.CharacterController->OldKnockback.Direction, filter.CharacterController->OriginalPosition);
            }
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

            if (filter.CharacterController->CurrentState == States.Jump)
                filter.CharacterController->NearbyColliders &= ~Colliders.Ground;

            // Get if the player is grounded or not...
            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = stats.Jump.AsShort;
                filter.CharacterController->DodgeCount = stats.Dodge.AsShort;
            }
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

        private void PreviewKnockback(FPVector2 amount, FPVector2 offset)
        {
            var lineList = CalculateArcPositions(20, amount, offset);

            for (int i = 0; i < lineList.Length - 1; ++i)
            {
                Draw.Line(lineList[i], lineList[i + 1]);
            }
        }

        private ReadOnlySpan<FPVector3> CalculateArcPositions(int resolution, FPVector2 amount, FPVector2 offset)
        {
            FPVector3[] positions = new FPVector3[resolution];

            for (int i = 0; i < resolution; ++i)
            {
                FP t = (FP)i / resolution;
                positions[i] = (CalculateArcPoint(t, 20, 1, amount) + offset).XYO;
            }

            return positions;
        }

        private FPVector2 CalculateArcPoint(FP t, FP gravity, FP scalar, FPVector2 amount)
        {
            amount.X += FP._1 / 10000;
            FP angle = FPMath.Atan2(amount.Y, amount.X);

            FP x = t * amount.X;
            FP y = x * FPMath.Tan(angle) - (gravity * x * x / (2 * amount.Magnitude * amount.Magnitude * FPMath.Cos(angle) * FPMath.Cos(angle)));

            return new FPVector2(x, y) * scalar;
        }
    }
}
