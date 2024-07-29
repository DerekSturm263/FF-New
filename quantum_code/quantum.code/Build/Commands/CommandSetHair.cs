using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetHair : DeterministicCommand
    {
        public EntityRef entity;
        public AssetRefHair hair;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref hair);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Hair applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetHair(f, entity, stats, hair);
        }
    }
}
