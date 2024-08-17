namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class HoldableItem : Item
    {
        public enum Handedness
        {
            OneHanded,
            TwoHanded
        }

        public Handedness HandednessType;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            ItemSystem.PickUp(f, user, item);
        }
    }
}
