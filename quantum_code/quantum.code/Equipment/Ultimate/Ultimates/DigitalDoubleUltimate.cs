namespace Quantum
{
    [System.Serializable]
    public unsafe partial class DigitalDoubleUltimate : Ultimate
    {
        public AssetRefEntityPrototype Prototype;
        public Bot Bot;
        public string Name;

        public override void OnBegin(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            base.OnBegin(f, ref filter);

            FighterIndex index = new()
            {
                Local = -1,
                Global = FighterIndex.GetNextGlobalIndex(f),
                GlobalNoBots = -1,
                GlobalNoHumans = FighterIndex.GetNextGlobalIndexNoHumans(f),
                Team = filter.PlayerStats->Index.Team,
                Device = filter.PlayerStats->Index.Device,
                Type = FighterType.Bot
            };

            EntityRef digitalDoubleEntity = PlayerSpawnSystem.SpawnPlayer(f, default, Prototype, false, Name, index, false, filter.PlayerStats->Build);

            if (f.Unsafe.TryGetPointer(digitalDoubleEntity, out CharacterController* characterController2))
                characterController2->Behavior = Bot.Behavior;

            filter.CharacterController->DigitalDouble = digitalDoubleEntity;
        }

        public override void OnEnd(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            if (filter.CharacterController->DigitalDouble.IsValid)
                PlayerSpawnSystem.DespawnPlayer(f, filter.CharacterController->DigitalDouble);
        }
    }
}
