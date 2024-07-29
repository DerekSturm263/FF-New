using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetHeadgear : DeterministicCommand
    {
        public EntityRef entity;
        public Apparel headgear;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref headgear);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Headgear applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetHeadgear(f, entity, stats, headgear);
        }
    }
}
