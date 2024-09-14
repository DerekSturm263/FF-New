using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UpdateableItem : Item
    {
        public int Lifetime;

        public override unsafe void Invoke(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter) { }

        public virtual unsafe void OnStart(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter)
        {
            if (AlignDirectionToVelocity)
                filter.Transform->Rotation = FPMath.Atan2(filter.PhysicsBody->Velocity.Y, filter.PhysicsBody->Velocity.X);
        }

        public abstract unsafe void OnUpdate(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter);
        public abstract unsafe void OnExit(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter);
    }
}
