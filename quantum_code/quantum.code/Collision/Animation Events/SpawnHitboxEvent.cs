using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnHitboxEvent : FrameEvent
    {
        public enum ParentType
        {
            Player,
            Weapon
        }

        public ParentType Parent;
        public Shape2DConfig Shape;

        public HitboxSettings UnchargedSettings;
        public HitboxSettings FullyChargedSettings;

        [HideInInspector] public ListHelper<FPVector2>[] BakedShapePositions;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning hitbox!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                for (int i = 0; i < Shape.CompoundShapes.Length; ++i)
                {
                    HitboxSettings settings = characterController->LerpFromAnimationHold(HitboxSettings.Lerp, UnchargedSettings, FullyChargedSettings);
                    HitboxSystem.SpawnHitbox(f, settings, Shape.CompoundShapes[i].CreateShape(f), Length, entity, BakedShapePositions[i].List);
                }
            }
        }
    }
}
