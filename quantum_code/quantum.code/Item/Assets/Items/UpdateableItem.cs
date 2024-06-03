namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class UpdateableItem : Item
    {
        public abstract unsafe void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef item, ItemInstance* itemInstance);
    }
}
