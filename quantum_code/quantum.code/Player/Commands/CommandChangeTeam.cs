using Photon.Deterministic;
using System.Linq;

namespace Quantum
{
    public unsafe class CommandChangeTeam : DeterministicCommand
    {
        public EntityRef player;
        public int teamIndex;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref player);
            stream.Serialize(ref teamIndex);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Team changed!");

            if (f.Unsafe.TryGetPointer(player, out CharacterController* characterController) && characterController->IsReady)
                return;

            if (f.Unsafe.TryGetPointer(player, out PlayerStats* stats))
            {
                stats->Index.Team = teamIndex;
                f.Events.OnPlayerChangeTeam(player, stats->Index);

                FighterIndex.UpdateGlobalList(f);
            }
        }
    }
}
