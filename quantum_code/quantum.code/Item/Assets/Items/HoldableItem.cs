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

        public override void Invoke(Frame f, ref CharacterControllerSystem.Filter user, ref ItemSystem.Filter filter)
        {
            ItemSystem.PickUp(f, user.Entity, filter.Entity);
        }
    }
}
