using Photon.Deterministic;
using Quantum.Types;

namespace Quantum
{
    public unsafe class CommandResetAllPlayerPositions : DeterministicCommand
    {
        public override void Serialize(BitStream stream)
        {

        }

        public void Execute(Frame f)
        {
            Log.Debug("Player positions reset!");

            var playerFilter = f.Unsafe.FilterStruct<CharacterControllerSystem.Filter>();
            var player = default(CharacterControllerSystem.Filter);

            while (playerFilter.Next(&player))
            {
                player.Transform->Position = ArrayHelper.All(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints)[player.PlayerStats->Index.Global];
            }
        }
    }
}
