using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSetBuild: DeterministicCommand
    {
        public EntityRef entity;
        public Build build;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref build);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Build applied!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController) && characterController->IsReady)
                return;

            if (f.Unsafe.TryGetPointer(entity, out PlayerStats* stats))
                PlayerStatsSystem.SetBuild(f, entity, stats, build);
        }
    }
}
