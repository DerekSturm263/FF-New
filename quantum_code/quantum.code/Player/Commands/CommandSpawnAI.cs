using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnAI : DeterministicCommand
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
            Log.Debug("AI spawned!");

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = behavior;
        }
    }
}
