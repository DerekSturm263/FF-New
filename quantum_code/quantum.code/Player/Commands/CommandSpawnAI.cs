using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandSpawnAI : DeterministicCommand
    {
        public AssetRefEntityPrototype prototype;
        public Bot bot;

        public string name;
        public FighterIndex index;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref prototype);
            stream.Serialize(ref bot);
            stream.Serialize(ref name);
            stream.Serialize(ref index);
        }

        public void Execute(Frame f)
        {
            Log.Debug("AI spawned!");

            EntityRef entity = PlayerSpawnSystem.SpawnPlayer(f, default, prototype, false, name, index, false, bot.Build);

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = bot.Behavior;
        }
    }
}
