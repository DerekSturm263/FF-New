using Photon.Deterministic;
using Quantum;
using System.Diagnostics;

public static class UserExtensions
{
    public static EntityRef CreateChilded(this Frame f, AssetRefEntityPrototype prototype, EntityRef parent)
    {
        EntityRef entity = f.Create(prototype);

        ChildParentLink link = new()
        {
            Parent = parent
        };
        f.Add(entity, link);

        return entity;
    }
}

namespace Quantum
{
    public static class DrawHelper
    {
        [Conditional("DEBUG")]
        public static unsafe void Shape3D(Frame f, Shape3D shape, FPVector3 position, FPQuaternion rotation, ColorRGBA? color = null)
        {
            if (shape.Compound.GetShapes(f, out Shape3D* shapesBuffer, out int count))
            {
                for (int i = 0; i < count; ++i)
                {
                    Shape3D* currentShape = shapesBuffer + i;
                    Shape3DChild(*currentShape, position, rotation, color);
                }
            }
        }

        [Conditional("DEBUG")]
        private static void Shape3DChild(Shape3D shape, FPVector3 position, FPQuaternion rotation, ColorRGBA? color = null)
        {
            var localOffset = shape.LocalTransform.Position;
            var localRotation = shape.LocalTransform.Rotation;

            position += rotation * localOffset;
            rotation *= localRotation;

            switch (shape.Type)
            {
                case Shape3DType.Sphere:
                    Draw.Sphere(position, shape.Sphere.Radius, color);
                    break;

                case Shape3DType.Box:
                    Draw.Box(position, shape.Box.Extents * 2, rotation, color);
                    break;
            }
        }
    }
}
