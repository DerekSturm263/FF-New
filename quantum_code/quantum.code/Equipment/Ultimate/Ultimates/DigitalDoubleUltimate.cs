namespace Quantum
{
    [System.Serializable]
    public unsafe partial class DigitalDoubleUltimate : Ultimate
    {
        public AssetRefEntityPrototype Prototype;
        public Bot Bot;
        public string Name;

        public override void OnBegin(Frame f, EntityRef user)
        {
            if (!f.TryGet(user, out PlayerStats playerStats))
                return;

            FighterIndex index = new()
            {
                Local = -1,
                Global = FighterIndex.GetNextGlobalIndex(f),
                GlobalNoBots = -1,
                GlobalNoHumans = FighterIndex.GetNextGlobalIndexNoHumans(f),
                Team = playerStats.Index.Team,
                Device = playerStats.Index.Device,
                Type = FighterType.Bot
            };

            EntityRef entity = PlayerSpawnSystem.SpawnPlayer(f, default, Prototype, false, Name, index, false, playerStats.Build);

            if (f.Unsafe.TryGetPointer(entity, out AIData* aiData))
                aiData->Behavior = Bot.Behavior;

            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
                characterController->DigitalDouble = entity;
        }

        public override void OnEnd(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out CharacterController* characterController) && characterController->DigitalDouble.IsValid)
                PlayerSpawnSystem.DespawnPlayer(f, characterController->DigitalDouble);
        }
    }
}
