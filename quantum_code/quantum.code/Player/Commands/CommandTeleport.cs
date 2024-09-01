using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandTeleport : DeterministicCommand
    {
        public EntityRef entity;
        public FPVector2 position;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref position);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Entity teleported!");

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
                transform->Position = position;
        }
    }
}
