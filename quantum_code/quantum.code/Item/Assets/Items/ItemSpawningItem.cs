namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ItemSpawningItem : UsableItem
    {
        public ItemSpawnSettings SpawnSettings;

        public override void Invoke(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            ItemSpawnSystem.Spawn(f, SpawnSettings, user);

            base.Invoke(f, user, ref filter);
        }
    }
}
