using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetBehavior : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefBehavior behavior;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref behavior);
        }

        public void Execute(Frame f)
        {
            Log.Debug("AI behavior set!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                characterController->Behavior = behavior;
        }
    }
}
