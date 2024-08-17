namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UpdateableItem : Item
    {
        public override unsafe void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance) { }

        public abstract unsafe void OnStart(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance);
        public abstract unsafe void OnUpdate(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance);
        public abstract unsafe void OnExit(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance);
    }
}
