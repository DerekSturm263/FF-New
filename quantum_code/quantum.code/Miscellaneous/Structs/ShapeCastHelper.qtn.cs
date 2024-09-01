using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial struct ShapeCastHelper
    {
        public LayerMask LayerMask;
        public Shape2DConfig Shape;

        public readonly Physics2D.HitCollection GetCastResults(Frame f, Transform2D* parent = null, FPVector2 offset = default)
        {
            if (parent is null)
            {
                if (Shape.ShapeType == Shape2DType.Box)
                    Draw.Rectangle(Shape.PositionOffset + offset, Shape.BoxExtents * 2, 0);
                else
                    Draw.Circle(Shape.PositionOffset + offset, Shape.CircleRadius);

                return f.Physics2D.OverlapShape(offset, 0, Shape.CreateShape(f), LayerMask);
            }
            else
            {
                if (Shape.ShapeType == Shape2DType.Box)
                    Draw.Rectangle(parent->Position + Shape.PositionOffset + offset, Shape.BoxExtents * 2, 0);
                else
                    Draw.Circle(parent->Position + Shape.PositionOffset + offset, Shape.CircleRadius);

                return f.Physics2D.OverlapShape(parent->Position + offset, 0, Shape.CreateShape(f), LayerMask);
            }
        }

        public readonly bool TryGetCastResults(Frame f, out Physics2D.HitCollection hitCollection, Transform2D* parent = null, FPVector2 offset = default)
        {
            hitCollection = GetCastResults(f, parent, offset);
            return hitCollection.Count > 0;
        }
    }
}
