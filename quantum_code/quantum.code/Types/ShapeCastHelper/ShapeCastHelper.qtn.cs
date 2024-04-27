﻿using Photon.Deterministic;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quantum
{
    [System.Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct ShapeCastHelper
    {
        public const int SIZE = 16;
        public const int ALIGNMENT = 8;

        [FieldOffset(0)]
        public LayerMask LayerMask;

        [FieldOffset(8)]
        public Shape2DConfig Shape;

        public override int GetHashCode()
        {
            int hashCode = 2098782172;
            hashCode = hashCode * -1521134295 + LayerMask.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Shape2DConfig>.Default.GetHashCode(Shape);
            return hashCode;
        }
        /*
        public static void Serialize(void* ptr, FrameSerializer serializer)
        {
            ShapeCastHelper* p = (ShapeCastHelper*)ptr;
            LayerMask.Serialize(&p->LayerMask, serializer);
            Shape2D.Serialize(&p->Shape, serializer);
        }
        */
        public readonly Physics2D.HitCollection GetCastResults(Frame f, Transform2D* parent = null)
        {
            if (parent is null)
            {
                if (Shape.ShapeType == Shape2DType.Box)
                    Draw.Rectangle(Shape.PositionOffset, Shape.BoxExtents, 0);
                else
                    Draw.Circle(Shape.PositionOffset, Shape.CircleRadius);

                return f.Physics2D.OverlapShape(FPVector2.Zero, 0, Shape.CreateShape(f), LayerMask);
            }
            else
            {
                if (Shape.ShapeType == Shape2DType.Box)
                    Draw.Rectangle(parent->Position + Shape.PositionOffset, Shape.BoxExtents, 0);
                else
                    Draw.Circle(parent->Position + Shape.PositionOffset, Shape.CircleRadius);

                return f.Physics2D.OverlapShape(parent->Position, 0, Shape.CreateShape(f), LayerMask);
            }
        }
    }
}
