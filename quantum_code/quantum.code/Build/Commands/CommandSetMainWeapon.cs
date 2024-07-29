using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetMainWeapon : DeterministicCommand
    {
        public EntityRef entity;
        public Weapon weapon;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref weapon);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Main Weapon applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetMainWeapon(f, entity, stats, weapon);
        }
    }
}
