using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetAltWeapon : DeterministicCommand
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
            Log.Debug("Alt Weapon applied!");

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetAltWeapon(f, entity, stats, weapon);
        }
    }
}
