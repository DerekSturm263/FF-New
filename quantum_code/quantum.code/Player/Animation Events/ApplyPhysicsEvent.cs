using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ApplyPhysicsEvent : FrameEvent
    {
        [System.Flags]
        public enum Usability : byte
        {
            Grounded = 1 << 0,
            Airborne = 1 << 1
        }

        public Usability UsabilityType;

        public PhysicsSettings UnchargedSettings;
        public PhysicsSettings FullyChargedSettings;

        public override void Begin(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Applying physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->GravityScale = 0;
            }

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController) && f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                characterController->ApplyPhysicsPosition = transform->Position;
            }
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame, int elapsedFrames)
        {
            Log.Debug("Updating physics!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController) && f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                PhysicsSettings settings = characterController->LerpFromAnimationHold(PhysicsSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                transform->Position = characterController->ApplyPhysicsPosition + GetPositionAtTime(settings, (FP)elapsedFrames / Length, characterController->MovementDirection);
            }
        }

        public static FPVector2 GetPositionAtTime(PhysicsSettings settings, FP normalizedTime, int direction)
        {
            return new FPVector2(settings.XCurve.Evaluate(normalizedTime) * settings.XForce * direction, settings.YCurve.Evaluate(normalizedTime) * settings.YForce);
        }

        public override void End(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->GravityScale = 1;
            }
        }
    }
}
