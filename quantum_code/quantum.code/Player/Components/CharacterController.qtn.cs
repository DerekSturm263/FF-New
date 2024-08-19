using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct CharacterController
    {
        public readonly bool IsHeldThisFrame(Input input, Input.Buttons button) => input.InputButtons.HasFlag(button);
        public readonly bool WasPressedThisFrame(Input input, Input.Buttons button) => input.InputButtons.HasFlag(button) && !LastFrame.InputButtons.HasFlag(button);
        public readonly bool WasReleasedThisFrame(Input input, Input.Buttons button) => !input.InputButtons.HasFlag(button) && LastFrame.InputButtons.HasFlag(button);

        public readonly Colliders GetNearbyColliders(Frame f, MovementSettings movementSettings, Transform2D* parent)
        {
            Colliders result = default;

            if (movementSettings.GroundedCast.GetCastResults(f, parent).Count > 0)
                result |= Colliders.Ground;
            if (movementSettings.WallCastLeft.GetCastResults(f, parent).Count > 0)
                result |= Colliders.LeftWall;
            if (movementSettings.WallCastRight.GetCastResults(f, parent).Count > 0)
                result |= Colliders.RightWall;
            if (movementSettings.CeilingCast.GetCastResults(f, parent).Count > 0)
                result |= Colliders.Ceiling;

            return result;
        }

        public readonly bool GetNearbyCollider(Colliders collider) => NearbyColliders.HasFlag(collider);

        public readonly MovementMoveSettings GetMoveSettings(MovementSettings movementSettings)
        {
            if (GetNearbyCollider(Colliders.Ground))
                return movementSettings.GroundedMoveSettings;
            else
                return movementSettings.AerialMoveSettings;
        }

        public readonly MovementCurveSettings GetJumpSettings(MovementSettings movementSettings)
        {
            return JumpType switch
            {
                JumpType.ShortHop => movementSettings.ShortHopSettings,
                JumpType.FullHop => movementSettings.FullHopSettings,
                JumpType.Aerial => movementSettings.AerialSettings,
                _ => default,
            };
        }

        public readonly MovementCurveSettingsXY GetDodgeSettings(MovementSettings movementSettings)
        {
            return DodgeType switch
            {
                DodgeType.Spot => movementSettings.SpotDodgeSettings,
                DodgeType.RollForward => movementSettings.ForwardRollSettings,
                DodgeType.RollBackward => movementSettings.BackwardRollSettings,
                DodgeType.Aerial => movementSettings.AerialDodgeSettings,
                _ => default,
            };
        }

        public void Move(Frame f, FP amount, ref CharacterControllerSystem.Filter filter, MovementSettings movementSettings, ApparelStats stats)
        {
            if (!CanInput)
                return;

            MovementMoveSettings moveSettings = GetMoveSettings(movementSettings);

            //bool isTurning = false;

            // Exit if the player is holding too much up or down.
            if (FPMath.Abs(amount) < movementSettings.DeadStickZone)
            {
                // Set the player to stop moving.
                Velocity = LerpSpeed(moveSettings, f.DeltaTime, 0, Velocity, moveSettings.Deceleration);
                MovingLerp = FPMath.Lerp(MovingLerp, 0, f.DeltaTime * 10);

                CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsMoving", false);
            }
            else
            {
                // Calculate the player's speed.
                FP topSpeed = CalculateTopSpeed(moveSettings, amount);

                // Apply the target velocity based on their speed.
                if (FPMath.Abs(Velocity) > (FP)1 / 20 && FPMath.SignInt(amount) != FPMath.SignInt(Velocity))
                {
                    Velocity = LerpSpeed(moveSettings, f.DeltaTime, amount, Velocity, moveSettings.TurnAroundSpeed);
                    //isTurning = true;
                }
                else if (FPMath.Abs(Velocity) < FPMath.Abs(topSpeed))
                {
                    Velocity = LerpSpeed(moveSettings, f.DeltaTime, amount, Velocity, moveSettings.Acceleration);
                }
                else if (FPMath.Abs(Velocity) > FPMath.Abs(topSpeed))
                {
                    Velocity = LerpSpeed(moveSettings, f.DeltaTime, amount, Velocity, moveSettings.Acceleration);
                }

                // Set the player's look direction.
                if (GetNearbyCollider(Colliders.Ground))
                {
                    if (MovementDirection == 1 && filter.PhysicsBody->Velocity.X < 0)
                    {
                        MovementDirection = -1;
                        f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, MovementDirection);
                    }
                    else if (MovementDirection == -1 && filter.PhysicsBody->Velocity.X > 0)
                    {
                        MovementDirection = 1;
                        f.Events.OnPlayerChangeDirection(filter.Entity, filter.PlayerStats->Index, MovementDirection);
                    }
                }

                MovingLerp = 1;
                CustomAnimator.SetBoolean(f, filter.CustomAnimator, "IsMoving", true);
            }

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "Speed", FPMath.Abs(Velocity / 10));

            filter.PhysicsBody->Velocity.X = (filter.CharacterController->Velocity * filter.CharacterController->Influence * stats.Agility) + filter.CharacterController->KnockbackVelocityX;
        }

        private readonly FP LerpSpeed(MovementMoveSettings settings, FP deltaTime, FP stickX, FP currentAmount, FP speedMultiplier) => FPMath.Lerp(currentAmount, CalculateTopSpeed(settings, stickX), deltaTime * speedMultiplier);
        private readonly FP CalculateTopSpeed(MovementMoveSettings settings, FP stickX) => stickX * settings.TopSpeed;

        public readonly T LerpFromAnimationHold<T>(System.Func<T, T, FP, T> lerpFunc, T a, T b) where T : struct
        {
            if (MaxHoldAnimationFrameTime > 0)
                return lerpFunc.Invoke(a, b, (FP)HeldAnimationFrameTime / MaxHoldAnimationFrameTime);
            else
                return a;
        }

        public readonly T LerpFromAnimationHold_UNSAFE<T>(System.Func<T, T, float, T> lerpFunc, T a, T b) where T : struct
        {
            if (MaxHoldAnimationFrameTime > 0)
                return lerpFunc.Invoke(a, b, (float)HeldAnimationFrameTime / MaxHoldAnimationFrameTime);
            else
                return a;
        }
    }
}
