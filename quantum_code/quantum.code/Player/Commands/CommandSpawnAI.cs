using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnAI : DeterministicCommand
    {
        public AssetRefEntityPrototype prototype;
        public Bot bot;
        public FighterIndex index;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref prototype);
            stream.Serialize(ref bot);
            stream.Serialize(ref index);
        }

        public void Execute(Frame f)
        {
            Log.Debug("AI spawned!");

            EntityRef entity = PlayerSpawnSystem.SpawnPlayer(f, default, prototype, false, "Bot", index);

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = bot.Behavior;

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                StatsSystem.SetBuild(f, entity, stats, bot.Build);
        }
    }
}
