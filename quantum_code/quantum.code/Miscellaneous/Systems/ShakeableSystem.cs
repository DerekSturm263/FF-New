using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ShakeableSystem : SystemMainThreadFilter<ShakeableSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public Shakeable* Shakeable;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.Shakeable->Time > 0)
            {
                ShakeSettings settings = f.FindAsset<ShakeSettings>(filter.Shakeable->Settings.Id);

                if (settings is not null)
                {
                    FP x = settings.RelativeHorizontalCurve.Evaluate(filter.Shakeable->Time / (FP._1 * filter.Shakeable->MaxTime)) * settings.Strength * filter.Shakeable->Strength;
                    FP y = settings.RelativeVerticalCurve.Evaluate(filter.Shakeable->Time / (FP._1 * filter.Shakeable->MaxTime)) * settings.Strength * filter.Shakeable->Strength;

                    filter.Shakeable->Amount = (filter.Shakeable->Direction * y) + (new FPVector2(filter.Shakeable->Direction.Y, -filter.Shakeable->Direction.X) * x);

                    filter.Transform->Position = filter.Shakeable->Position + filter.Shakeable->Amount;
                }

                --filter.Shakeable->Time;
                if (filter.Shakeable->Time <= 0)
                {
                    filter.Transform->Position = filter.Shakeable->Position;

                    if (f.Unsafe.TryGetPointer(filter.Entity, out CustomAnimator* customAnimator))
                    {
                        customAnimator->speed = FP._1;
                    }
                }
            }
            else
            {
                filter.Shakeable->Amount = default;
            }
        }

        public static void Shake(Frame f, EntityRef shakeableEntity, AssetRefShakeSettings settings, FPVector2 direction, uint time, FP strength)
        {
            if (time == 0)
                return;

            if (f.Unsafe.TryGetPointer(shakeableEntity, out Shakeable* shakeable) && f.Unsafe.TryGetPointer(shakeableEntity, out Transform2D* transform))
            {
                shakeable->Settings = settings;

                shakeable->Time = (int)time;
                shakeable->MaxTime = (int)time;

                shakeable->Strength = strength;
                shakeable->Position = transform->Position;
                shakeable->Direction = direction;

                if (f.Unsafe.TryGetPointer(shakeableEntity, out CustomAnimator* customAnimator))
                {
                    customAnimator->speed = 0;
                }
            }
        }
    }
}