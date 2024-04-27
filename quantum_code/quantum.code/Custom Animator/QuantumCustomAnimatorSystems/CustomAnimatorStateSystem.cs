﻿
using Quantum.Custom.Animator;
using System.Diagnostics;

namespace Quantum
{
    public unsafe class CustomAnimatorStateSystem : SystemSignalsOnly, ISignalOnAnimatorStateEnter, ISignalOnAnimatorStateUpdate, ISignalOnAnimatorStateExit
    {
        public static void Initilize(Frame f, CustomAnimator* animator)
        {

        }

        public void OnAnimatorStateEnter(Frame f, EntityRef entity, CustomAnimator* animator)
        {
        }

        public void OnAnimatorStateUpdate(Frame f, EntityRef entity, CustomAnimator* animator)
        {
            AnimatorState state = CustomAnimator.GetCurrentState(f, animator);

            if (f.TryFindAsset(state.StateAsset.Id, out QuantumAnimationEvent asset))
            {
                int frame = 0;

                AnimatorMotion motion = state.motion;
                frame = (int)(animator->normalized_time * motion.CalculateLength(f, animator, 1, state) * 60);

                asset.InvokeEvents(f, entity, frame);
            }
        }

        public void OnAnimatorStateExit(Frame f, EntityRef entity, CustomAnimator* animator)
        {
        }
    }
}