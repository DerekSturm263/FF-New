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

        public readonly bool GetIsGrounded(Frame f, MovementSettings movementSettings, Transform2D* parent)
        {
            return movementSettings.GroundedCast.GetCastResults(f, parent).Count > 0;
        }

        public readonly int GetIsAgainstWall(Frame f, MovementSettings movementSettings, Transform2D* parent)
        {
            if (movementSettings.WallCastLeft.GetCastResults(f, parent).Count > 0)
                return -1;
            else if (movementSettings.WallCastRight.GetCastResults(f, parent).Count > 0)
                return 1;

            return 0;
        }

        public readonly MovementMoveSettings GetMoveSettings(MovementSettings movementSettings, bool isGrounded)
        {
            if (isGrounded)
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

        public readonly MovementCurveSettings GetDodgeSettings(MovementSettings movementSettings, bool isGrounded)
        {
            return DodgeSettingsIndex switch
            {
                0 => movementSettings.GroundedDodgeSettings,
                1 => movementSettings.AerialDodgeSettings,
                _ => default,
            };
        }

        public void Move(Frame f, FP amount, Transform2D* transform, PhysicsBody2D* physicsBody, CustomAnimator* customAnimator, MovementSettings movementSettings, bool isGrounded, FP dt)
        {
            MovementMoveSettings moveSettings = GetMoveSettings(movementSettings, isGrounded);

            //bool isTurning = false;

            // Exit if the player is holding too much up or down.
            if (FPMath.Abs(amount) < movementSettings.DeadStickZone)
            {
                // Set the player to stop moving.
                Velocity = LerpSpeed(moveSettings, dt, 0, Velocity, moveSettings.Deceleration);
                MovingLerp = FPMath.Lerp(MovingLerp, 0, dt * 10);
            }
            else
            {
                // Calculate the player's speed.
                FP topSpeed = CalculateTopSpeed(moveSettings, amount);

                // Apply the target velocity based on their speed.
                if (FPMath.Abs(Velocity) > (FP)1 / 20 && FPMath.Sign(amount) != FPMath.Sign(Velocity))
                {
                    Velocity = LerpSpeed(moveSettings, dt, amount, Velocity, moveSettings.TurnAroundSpeed);
                    //isTurning = true;
                }
                else if (FPMath.Abs(Velocity) < FPMath.Abs(topSpeed))
                {
                    Velocity = LerpSpeed(moveSettings, dt, amount, Velocity, moveSettings.Acceleration);
                }
                else if (FPMath.Abs(CalculateTopSpeed(moveSettings, amount)) < FPMath.Abs(Velocity))
                {
                    Velocity = LerpSpeed(moveSettings, dt, amount, Velocity, moveSettings.Acceleration);
                }

                // Set the player's look direction.
                if (isGrounded)
                {
                    if (physicsBody->Velocity.X < 0)
                    {
                        //transform->Rotation = FPQuaternion.Euler(movementSettings.FacingLeftDirection);
                    }
                    else if (physicsBody->Velocity.X > 0)
                    {
                        //transform->Rotation = FPQuaternion.Euler(movementSettings.FacingRightDirection);
                    }
                }

                MovingLerp = 1;
            }

            // Apply velocity to the player.
            physicsBody->Velocity.X = Velocity;

            CustomAnimator.SetFixedPoint(f, customAnimator, "Speed", FPMath.Abs(Velocity));
        }

        private readonly FP LerpSpeed(MovementMoveSettings settings, FP deltaTime, FP stickX, FP currentAmount, FP speedMultiplier) => FPMath.Lerp(currentAmount, CalculateTopSpeed(settings, stickX), deltaTime * speedMultiplier);
        private readonly FP CalculateTopSpeed(MovementMoveSettings settings, FP stickX) => stickX * settings.TopSpeed;
    }
}
