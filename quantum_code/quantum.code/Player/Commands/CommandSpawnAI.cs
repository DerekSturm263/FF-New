using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnAI : DeterministicCommand
    {
        public AssetRefEntityPrototype prototype;
        public AssetRefBehavior behavior;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref prototype);
            stream.Serialize(ref behavior);
        }

        public void Execute(Frame f)
        {
            Log.Debug("AI spawned!");

            EntityRef entity = PlayerSpawnSystem.SpawnPlayer(f, default, prototype);

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = behavior;
        }
    }
}
