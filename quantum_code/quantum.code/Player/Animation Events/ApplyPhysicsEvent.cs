using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public struct PhysicsSettings
    {
        public FPAnimationCurve XCurve;
        public FP XForce;
        public FPAnimationCurve YCurve;
        public FP YForce;

        public static PhysicsSettings Lerp(PhysicsSettings a, PhysicsSettings b, FP t)
        {
            return new()
            {
                XCurve = Lerp(a.XCurve, b.XCurve, t),
                XForce = FPMath.Lerp(a.XForce, b.XForce, t),
                YCurve = Lerp(a.YCurve, b.YCurve, t),
                YForce = FPMath.Lerp(a.YForce, b.YForce, t)
            };
        }

        public static FPAnimationCurve Lerp(FPAnimationCurve a, FPAnimationCurve b, FP t)
        {
            FP[] samples = new FP[a.Samples.Length];
            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = FPMath.Lerp(a.Samples[i], b.Samples[i], t);
            }

            FPAnimationCurve.Keyframe[] keys = new FPAnimationCurve.Keyframe[a.Keys.Length];
            for (int i = 0; i < keys.Length; ++i)
            {
                keys[i] = new()
                {
                    Time = FPMath.Lerp(a.Keys[i].Time, b.Keys[i].Time, t),
                    Value = FPMath.Lerp(a.Keys[i].Value, b.Keys[i].Value, t),
                    InTangent = FPMath.Lerp(a.Keys[i].InTangent, b.Keys[i].InTangent, t),
                    OutTangent = FPMath.Lerp(a.Keys[i].OutTangent, b.Keys[i].OutTangent, t),
                    TangentMode = a.Keys[i].TangentMode,
                    TangentModeLeft = a.Keys[i].TangentModeLeft,
                    TangentModeRight = a.Keys[i].TangentModeRight
                };
            }

            return new()
            {
                Samples = samples,
                PreWrapMode = a.PreWrapMode,
                PostWrapMode = a.PostWrapMode,
                StartTime = FPMath.Lerp(a.StartTime, b.StartTime, t),
                EndTime = FPMath.Lerp(a.EndTime, b.EndTime, t),
                Resolution = a.Resolution,
                OriginalPreWrapMode = b.OriginalPreWrapMode,
                OriginalPostWrapMode = a.OriginalPostWrapMode,
                Keys = keys
            };
        }
    }

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

        public PhysicsSettings Settings;
        public PhysicsSettings MaxHoldSettings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Applying physics!");

            if (f.Unsafe.TryGetPointer(entity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->GravityScale = 0;
            }
        }

        public override void Update(Frame f, EntityRef entity, int frame, int elapsedFrames)
        {
            Log.Debug("Updating physics!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController) && f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                PhysicsSettings settings = characterController->LerpFromAnimationHold(PhysicsSettings.Lerp, Settings, MaxHoldSettings);
                transform->Position = GetPositionAtTime(settings, (FP)elapsedFrames / Length);
            }
        }

        public static FPVector2 GetPositionAtTime(PhysicsSettings settings, FP normalizedTime)
        {
            return new FPVector2(settings.XCurve.Evaluate(normalizedTime) * settings.XForce, settings.YCurve.Evaluate(normalizedTime) * settings.YForce);
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
