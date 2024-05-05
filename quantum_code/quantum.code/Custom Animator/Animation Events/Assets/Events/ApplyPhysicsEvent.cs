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

        public enum ForceType : byte
        {
            SetVelocity,
            AddForceForce,
            AddLinearImpulse
        }

        public Usability UsabilityType;
        public ForceType AppliedForceType;
        public FPAnimationCurve MotionCurve;
        public FP MotionForce;
        public FP GravityScale;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Applying physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->GravityScale = GravityScale;
            }
        }

        public override void Update(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Updating physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                FPVector2 force = new FPVector2(frame, MotionCurve.Evaluate(frame)) * MotionForce;

                switch (AppliedForceType)
                {
                    case ForceType.SetVelocity:
                        physicsBody->Velocity = force;
                        break;

                    case ForceType.AddForceForce:
                        physicsBody->AddForce(force);
                        break;

                    case ForceType.AddLinearImpulse:
                        physicsBody->AddLinearImpulse(force);
                        break;

                    default:
                        break;
                }
            }
        }

        public override void End(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->GravityScale = 1;
            }
        }
    }
}
