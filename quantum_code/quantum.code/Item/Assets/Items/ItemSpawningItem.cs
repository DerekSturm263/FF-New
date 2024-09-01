namespace Quantum
{
    [System.Serializable]
    public unsafe partial class ItemSpawningItem : UsableItem
    {
        public ItemSpawnSettings SpawnSettings;
        public int MaxUses;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            ItemSpawnSystem.Spawn(f, SpawnSettings, user);

            base.Invoke(f, user, item, itemInstance);
        }
    }
}
