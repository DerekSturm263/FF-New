using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quantum.Movement.MovementSystem;

namespace Quantum
{
    public abstract unsafe class AnimationBehavior : CustomAnimatorBehaviour
    {
        public override unsafe void OnEnter(Frame f, EntityRef entity, CustomAnimator* animator)
        {

        }

        public override unsafe bool OnUpdate(Frame f, EntityRef entity, CustomAnimator* animator)
        {
            return false;
        }

        public override unsafe void OnExit(Frame f, EntityRef entity, CustomAnimator* animator)
        {

        }
    }
}
