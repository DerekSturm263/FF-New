using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ChildParentLinkSystem : SystemMainThreadFilter<ChildParentLinkSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public ChildParentLink* ChildParentLink;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            FPMatrix4x4 parent = FPMatrix4x4.Identity;
            FPMatrix4x4 child = FPMatrix4x4.Identity;

            FPMatrix4x4 result = FPMatrix4x4.Identity;

            if (f.Unsafe.TryGetPointer(filter.ChildParentLink->Parent, out Transform2D* parentTransform2D))
            {
                parent = FPMatrix4x4.TRS(parentTransform2D->Position.XYO, FPQuaternion.Euler(0, 0, parentTransform2D->Rotation), FPVector3.One);
                child = FPMatrix4x4.TRS(-filter.ChildParentLink->LocalPosition, filter.ChildParentLink->LocalRotation, FPVector3.One);

                result = parent.Inverted * child;

            }
            else if (f.Unsafe.TryGetPointer(filter.ChildParentLink->Parent, out Transform3D* parentTransform3D))
            {
                parent = FPMatrix4x4.TRS(parentTransform3D->Position, parentTransform3D->Rotation, FPVector3.One);
                child = FPMatrix4x4.TRS(-filter.ChildParentLink->LocalPosition, filter.ChildParentLink->LocalRotation, FPVector3.One);

                result = parent.Inverted * child;
            }

            if (f.Unsafe.TryGetPointer(filter.Entity, out Transform2D* childTransform2D))
            {
                childTransform2D->Position = new(-result.M03, -result.M13);
                childTransform2D->Rotation = result.Rotation.Z;
            }
            else if (f.Unsafe.TryGetPointer(filter.Entity, out Transform3D* childTransform3D))
            {
                childTransform3D->Position = new(-result.M03, -result.M13, -result.M23);
                childTransform3D->Rotation = result.Rotation;
            }
        }
    }
}
