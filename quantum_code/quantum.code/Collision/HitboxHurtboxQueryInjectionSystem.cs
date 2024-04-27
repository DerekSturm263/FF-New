﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Collision
{
    public unsafe class HitboxHurtboxQueryInjectionSystem : SystemMainThread
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* Hitbox;
        }

        public override void Update(Frame f)
        {
            var hitboxFilter = f.Unsafe.FilterStruct<Filter>();
            var hitbox = default(Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Parent, out Transform2D* transform))
                {
                    hitbox.Hitbox->PathQueryIndex = f.Physics2D.AddOverlapShapeQuery
                    (
                        position: hitbox.Transform->Position + transform->Position,
                        rotation: 0,
                        shape: Shape2D.CreateCircle(hitbox.Hitbox->Settings.Radius, hitbox.Hitbox->Settings.Offset),
                        layerMask: hitbox.Hitbox->Settings.Layer
                    );
                }
            }
        }
    }
}
