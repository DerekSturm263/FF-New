using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnAI : DeterministicCommand
    {
        public AssetRefBehavior behavior;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref behavior);
        }

        public void Execute(Frame f)
        {
            Log.Debug("AI spawned!");

            EntityRef entity = PlayerSpawnSystem.SpawnPlayer(f, default);

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = behavior;
        }
    }
}
