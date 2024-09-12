namespace Quantum
{
    [System.Serializable]
    public unsafe partial class BouncySubEnhancer : SubEnhancer
    {
        public int MaxBounces;
        public LayerMask Wall;
        public AssetRefPhysicsMaterial Material;

        public override void OnSpawn(Frame f, ref CharacterControllerSystem.Filter filter, EntityRef subWeapon, Sub sub)
        {
            if (f.Unsafe.TryGetPointer(subWeapon, out ItemInstance* itemInstance))
            {
                itemInstance->MaxCollisions = MaxBounces;
                itemInstance->DoBounce = true;
            }

            if (f.Unsafe.TryGetPointer(subWeapon, out PhysicsCollider2D* physicsCollider))
            {
                physicsCollider->Material = Material;
            }
        }
    }
}
