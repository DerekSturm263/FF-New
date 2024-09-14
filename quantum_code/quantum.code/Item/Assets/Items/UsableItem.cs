namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UsableItem : Item
    {
        public int Uses;

        public override void Invoke(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter)
        {
            ++filter.ItemInstance->Uses;

            f.Events.OnItemUse(user.Entity, filter.Entity, this, filter.Transform->Position);

            if (filter.ItemInstance->Uses == Uses)
            {
                ItemSpawnSystem.Despawn(f, filter.Entity);
            }
        }
    }
}
