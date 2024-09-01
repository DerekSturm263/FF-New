namespace Quantum
{
    [System.Serializable]
    public unsafe partial class HitboxItem : Item
    {
        public HitboxSettings HitboxSettings;
        public Shape2DConfig Shape;

        public override void Invoke(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {

        }
    }
}
