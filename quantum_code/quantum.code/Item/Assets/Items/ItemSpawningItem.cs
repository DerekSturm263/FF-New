namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ItemSpawningItem : UsableItem
    {
        public ItemSpawnSettings SpawnSettings;

        public override void Invoke(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter)
        {
            ItemSpawnSystem.Spawn(f, SpawnSettings, user.Entity);

            base.Invoke(f, ref user, ref filter);
        }
    }
}
