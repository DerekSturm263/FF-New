using Photon.Deterministic;
using System;

namespace Quantum
{
    public unsafe class CharacterControllerSystem : SystemMainThreadFilter<CharacterControllerSystem.Filter>, ISignalOnComponentAdded<CharacterController>
    {
        public static PlayerStateMachine StateMachine = new();

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

        public void OnAdded(Frame f, EntityRef entity, CharacterController* component)
        {
            if (f.Unsafe.TryGetPointer(entity, out CustomAnimator* customAnimator))
                CustomAnimator.SetBoolean(f, customAnimator, (int)States.Default, true);
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Grab the player's behavior and movement settings.
            Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
            MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->Settings.Id);

            // Get the entity's input before we do anything with it.
            Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(filter.Entity).Player) : behavior.GetInput(f, filter);

            // Handle some miscellaneous logic.
            HandleGround(f, filter, settings);
            HandleRespawning(f, ref filter, settings);
            HandleButtonHolding(f, ref filter, input);
            HandleUltimate(f, filter);

            // Resolve the State Machine to determine which state the player should be in.
            StateMachine.Resolve(f, ref filter, input, settings);

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

                if (filter.CharacterController->CurrentKnockback.Time <= 0)
                {
                    filter.CharacterController->CurrentKnockback = default;
                }

                PreviewKnockback(filter.CharacterController->OldKnockback.Direction, filter.CharacterController->OriginalPosition);
            }
        }

        private void HandleGround(Frame f, Filter filter, MovementSettings movementSettings)
        {
            ApparelStats stats = ApparelHelper.FromStats(f, filter.PlayerStats);

            // Get all the nearby colliders.
            filter.CharacterController->NearbyColliders = filter.CharacterController->GetNearbyColliders(f, movementSettings, filter.Transform);
            filter.CharacterController->NearbyColliders &= ~f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).MutuallyExclusiveColliders;

            // Get if the player is grounded or not...
            if (filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                // If they are, reset their jumps and dodges.
                filter.CharacterController->JumpCount = stats.Jump.AsShort;
                filter.CharacterController->DodgeCount = stats.Dodge.AsShort;
            }
        }

        private void HandleRespawning(Frame f, ref Filter filter, MovementSettings movementSettings)
        {
            if (!filter.Stats->IsRespawning)
                return;

            StatsSystem.ModifyHealth(f, filter.Entity, filter.Stats, movementSettings.RespawnHealRate, false);

            if (filter.Stats->CurrentStats.Health >= f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
            {
                filter.Stats->IsRespawning = false;
                StatsSystem.ModifyHurtboxes(f, filter.Entity, (HurtboxType)32767, new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 }, true);
            }
        }

        private void HandleButtonHolding(Frame f, ref Filter filter, Input input)
        {
            if (filter.CharacterController->HoldButton == -1)
                return;

            bool buttonHeld = filter.CharacterController->IsHeldThisFrame(input, (Input.Buttons)filter.CharacterController->HoldButton);

            if (buttonHeld)
            {
                ++filter.CharacterController->HeldAnimationFrameTime;
            }

            if (!buttonHeld || filter.CharacterController->HeldAnimationFrameTime >= filter.CharacterController->MaxHoldAnimationFrameTime)
                filter.CustomAnimator->speed = 1;
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

        public static void ApplyKnockback(Frame f, HitboxSettings hitbox, EntityRef attacker, EntityRef defender, int directionMultiplier)
        {
            FPVector2 updatedDirection = new(hitbox.Offensive.Knockback.X * directionMultiplier, hitbox.Offensive.Knockback.Y);

            ShakeableSystem.Shake(f, attacker, hitbox.Visual.TargetShake, updatedDirection, hitbox.Delay.UserFreezeFrames, 0);
            ShakeableSystem.Shake(f, defender, hitbox.Visual.TargetShake, updatedDirection, hitbox.Delay.TargetFreezeFrames, hitbox.Delay.TargetShakeStrength);

            if (f.Unsafe.TryGetPointer(defender, out PhysicsBody2D* physicsBody) && f.Unsafe.TryGetPointer(defender, out CharacterController* characterController) && f.Unsafe.TryGetPointer(defender, out Transform2D* transform))
            {
                characterController->DeferredKnockback = new() { Direction = updatedDirection, Time = hitbox.Offensive.Knockback.Magnitude / 12 };
                characterController->OldKnockback = characterController->DeferredKnockback;
                characterController->OriginalPosition = transform->Position;

                if (f.TryGet(defender, out PlayerStats stats))
                {
                    characterController->MovementDirection = -FPMath.SignInt(updatedDirection.X);
                    f.Events.OnPlayerChangeDirection(defender, stats.Index, characterController->MovementDirection);
                }
            }
        }
    }
}
