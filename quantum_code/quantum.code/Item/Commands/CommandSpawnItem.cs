using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnItem : DeterministicCommand
    {
        public ItemSpawnSettings settings;
        public EntityRef owner;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref settings);
            stream.Serialize(ref owner);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Item spawned!");

            if (settings.StartHolding)
                ItemSpawnSystem.SpawnParented(f, settings, owner);
            else
                ItemSpawnSystem.Spawn(f, settings);
        }
    }
}
