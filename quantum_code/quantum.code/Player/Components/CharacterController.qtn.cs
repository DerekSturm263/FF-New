using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct CharacterController
    {
        public void SetState(States state, bool isSet)
        {
            if (isSet)
                States.Set((int)state);
            else
                States.Clear((int)state);
        }

        public readonly bool IsInState(States state) => States.IsSet((int)state);

        public void SetIsHolding(States state, bool isHolding)
        {
            if (isHolding)
                Holding.Set((int)state);
            else
                Holding.Clear((int)state);
        }

        public readonly bool IsHolding(States state) => Holding.IsSet((int)state);

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
            return JumpSettingsIndex switch
            {
                0 => movementSettings.ShortHopSettings,
                1 => movementSettings.FullHopSettings,
                2 => movementSettings.AerialJumpSettings,
                3 => movementSettings.WallJumpSettings,
                _ => default,
            };
        }

        public readonly MovementCurveSettings GetDodgeSettings(MovementSettings movementSettings)
        {
            return DodgeSettingsIndex switch
            {
                0 => movementSettings.GroundedDodgeSettings,
                1 => movementSettings.AerialDodgeSettings,
                _ => default,
            };
        }

        public void Move(Frame f, FP amount, ref CharacterControllerSystem.Filter filter, MovementSettings movementSettings, ApparelStats stats)
        {
            MovementMoveSettings moveSettings = GetMoveSettings(movementSettings);

            //bool isTurning = false;

            // Exit if the player is holding too much up or down.
            if (FPMath.Abs(amount) < movementSettings.DeadStickZone)
            {
                // Set the player to stop moving.
                Velocity = LerpSpeed(moveSettings, f.DeltaTime, 0, Velocity, moveSettings.Deceleration);
                MovingLerp = FPMath.Lerp(MovingLerp, 0, f.DeltaTime * 10);
            }
            else
            {
                // Calculate the player's speed.
                FP topSpeed = CalculateTopSpeed(moveSettings, amount);

                // Apply the target velocity based on their speed.
                if (FPMath.Abs(Velocity) > (FP)1 / 20 && FPMath.Sign(amount) != FPMath.Sign(Velocity))
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
                        f.Events.OnPlayerChangeDirection(*filter.PlayerLink, MovementDirection);
                    }
                    else if (MovementDirection == -1 && filter.PhysicsBody->Velocity.X > 0)
                    {
                        MovementDirection = 1;
                        f.Events.OnPlayerChangeDirection(*filter.PlayerLink, MovementDirection);
                    }
                }

                MovingLerp = 1;
            }

            // Apply velocity to the player.
            filter.PhysicsBody->Velocity.X = Velocity * stats.Agility;

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "Speed", FPMath.Abs(Velocity));
        }

        private readonly FP LerpSpeed(MovementMoveSettings settings, FP deltaTime, FP stickX, FP currentAmount, FP speedMultiplier) => FPMath.Lerp(currentAmount, CalculateTopSpeed(settings, stickX), deltaTime * speedMultiplier);
        private readonly FP CalculateTopSpeed(MovementMoveSettings settings, FP stickX) => stickX * settings.TopSpeed;
    }
}
