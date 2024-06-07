using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandDespawnPlayer : DeterministicCommand
    {
        public EntityRef entity;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Player despawned!");

            PlayerSpawnSystem.DespawnPlayer(f, entity);
        }
    }
}
