using Quantum.Custom.Animator;

namespace Quantum
{
    public unsafe class CustomAnimatorStateSystem : SystemSignalsOnly, ISignalOnAnimatorStateEnter, ISignalOnAnimatorStateUpdate, ISignalOnAnimatorStateExit, ISignalOnComponentAdded<CustomAnimator>, ISignalOnComponentRemoved<CustomAnimator>
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
                AnimatorMotion motion = state.motion;
                int frame = (animator->normalized_time * motion.CalculateLength(f, animator, 1, state) * 60).AsInt;

                Log.Debug($"Frame: {frame}");

                if (f.Unsafe.ComponentGetter<CharacterControllerSystem.Filter>().TryGet(f, entity, out CharacterControllerSystem.Filter filter))
                {
                    Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
                    Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

                    asset.InvokeEvents(f, ref filter, input, frame);
                }
            }
        }

        public void OnAnimatorStateExit(Frame f, EntityRef entity, CustomAnimator* animator)
        {
            if (f.Unsafe.ComponentGetter<CharacterControllerSystem.Filter>().TryGet(f, entity, out CharacterControllerSystem.Filter filter))
            {
                var list = f.ResolveList(animator->UnresolvedEvents);

                while (list.Count > 0)
                {
                    FrameEvent animEvent = f.FindAsset<FrameEvent>(list[0].Id);

                    Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
                    Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

                    animEvent.End(f, default, ref filter, input, 0);
                    list.RemoveAt(0);
                }

                if (filter.CharacterController->ResetActions)
                {
                    filter.CharacterController->ResetActions = false;

                    filter.CharacterController->MaintainVelocity = false;
                    filter.CharacterController->CanMove = true;
                    filter.CharacterController->PossibleStates = (StatesFlag)((int)StatesFlag.KnockedOver * 2 - 1);
                }
            }
        }

        public void OnAdded(Frame f, EntityRef entity, CustomAnimator* component)
        {
            component->UnresolvedEvents = f.AllocateList<AssetRefFrameEvent>();
        }

        public void OnRemoved(Frame f, EntityRef entity, CustomAnimator* component)
        {
            f.FreeList(component->UnresolvedEvents);
            component->UnresolvedEvents = default;
        }
    }
}