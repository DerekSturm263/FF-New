using Photon.Deterministic;
using System;
using System.Diagnostics;

namespace Quantum
{
    public unsafe class CharacterControllerSystem : SystemMainThreadFilter<CharacterControllerSystem.Filter>, ISignalOnComponentAdded<PlayerLink>
    {
        public static PlayerStateMachine StateMachine = new();

        public struct Filter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public PhysicsCollider2D* PhysicsCollider;
            public CustomAnimator* CustomAnimator;
            public PlayerStats* PlayerStats;
            public Stats* Stats;
            public Shakeable* Shakeable;
        }

        public void OnAdded(Frame f, EntityRef entity, PlayerLink* component)
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

            if (!filter.CharacterController->NearbyColliders.HasFlag(Colliders.Ground))
            {
                filter.PhysicsCollider->Layer = movementSettings.NoPlayerCollision;
            }
            else if (filter.CharacterController->NearbyColliders.HasFlag(Colliders.Ground) && !f.FindAsset<PlayerState>(filter.CharacterController->CurrentState.Id).DisableCollision)
            {
                filter.PhysicsCollider->Layer = movementSettings.PlayerCollision;
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
            if (filter.CharacterController->HoldButton == 0)
                return;

            bool buttonHeld = filter.CharacterController->IsHeldThisFrame(input, (Input.Buttons)filter.CharacterController->HoldButton);

            if (buttonHeld)
            {
                ++filter.CharacterController->HeldAnimationFrameTime;
            }

            if (!buttonHeld || filter.CharacterController->HeldAnimationFrameTime >= filter.CharacterController->MaxHoldAnimationFrameTime)
            {
                if (filter.PlayerStats->ActiveWeapon == ActiveWeaponType.Primary)
                    filter.CustomAnimator->speed = f.FindAsset<WeaponMaterial>(filter.PlayerStats->Build.Gear.MainWeapon.Material.Id).Stats.Speed;
                else
                    filter.CustomAnimator->speed = f.FindAsset<WeaponMaterial>(filter.PlayerStats->Build.Gear.AltWeapon.Material.Id).Stats.Speed;
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

        public static void ApplyKnockback(Frame f, HitboxSettings hitbox, EntityRef attacker, EntityRef defender, int directionMultiplier, FP freezeFramesMultiplier, int hitboxLifetime)
        {
            FP newX = hitbox.Offensive.Knockback.X * directionMultiplier;
            FP newY = (hitbox.Offensive.Knockback.X == FP._0 && hitbox.Offensive.Knockback.Y < 0 && f.Get<CharacterController>(defender).GetNearbyCollider(Colliders.Ground)) ? -hitbox.Offensive.Knockback.Y : hitbox.Offensive.Knockback.Y;

            FPVector2 updatedDirection = FPVector2.Scale(new(newX, newY), f.Unsafe.GetPointer<CharacterController>(defender)->KnockbackMultiplier);

            uint freezeTime = (uint)(hitbox.Delay.FreezeFrames * freezeFramesMultiplier).AsInt;

            ShakeableSystem.Shake(f, attacker, hitbox.Visual.TargetShake, updatedDirection, hitbox.Delay.FreezeFrames, 0);
            ShakeableSystem.Shake(f, defender, hitbox.Visual.TargetShake, updatedDirection, freezeTime, hitbox.Delay.ShakeStrength);

            if (f.Unsafe.TryGetPointer(defender, out Stats* stats))
            {
                stats->IFrameTime = (int)freezeTime + hitboxLifetime;

                if (f.Unsafe.TryGetPointer(defender, out PhysicsBody2D* physicsBody) && f.Unsafe.TryGetPointer(defender, out CharacterController* characterController) && f.Unsafe.TryGetPointer(defender, out Transform2D* transform))
                {
                    characterController->DeferredKnockback = new() { Direction = updatedDirection };
                    characterController->OldKnockback = characterController->DeferredKnockback;
                    characterController->OriginalPosition = transform->Position;
                    characterController->HitStunTime = hitbox.Offensive.HitStun;

                    if (updatedDirection != FPVector2.Zero && f.TryGet(defender, out PlayerStats playerStats))
                    {
                        characterController->MovementDirection = -FPMath.SignInt(updatedDirection.X);
                        f.Events.OnPlayerChangeDirection(defender, playerStats.Index, characterController->MovementDirection);
                    }
                }
            }
        }
    }
}
