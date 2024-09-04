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

                CharacterControllerSystem.Filter filter = new()
                {
                    Entity = entity,
                    Transform = f.Unsafe.GetPointer<Transform2D>(entity),
                    CharacterController = f.Unsafe.GetPointer<CharacterController>(entity),
                    CustomAnimator = animator,
                    PhysicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(entity),
                    PhysicsCollider = f.Unsafe.GetPointer<PhysicsCollider2D>(entity),
                    PlayerStats = f.Unsafe.GetPointer<PlayerStats>(entity),
                    Shakeable = f.Unsafe.GetPointer<Shakeable>(entity),
                    Stats = f.Unsafe.GetPointer<Stats>(entity)
                };

                Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
                Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

                asset.InvokeEvents(f, ref filter, input, frame);
            }
        }

        public void OnAnimatorStateExit(Frame f, EntityRef entity, CustomAnimator* animator)
        {
            var list = f.ResolveList(animator->UnresolvedEvents);
            
            while (list.Count > 0)
            {
                FrameEvent animEvent = f.FindAsset<FrameEvent>(list[0].Id);

                CharacterControllerSystem.Filter filter = new()
                {
                    Entity = entity,
                    Transform = f.Unsafe.GetPointer<Transform2D>(entity),
                    CharacterController = f.Unsafe.GetPointer<CharacterController>(entity),
                    CustomAnimator = animator,
                    PhysicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(entity),
                    PhysicsCollider = f.Unsafe.GetPointer<PhysicsCollider2D>(entity),
                    PlayerStats = f.Unsafe.GetPointer<PlayerStats>(entity),
                    Shakeable = f.Unsafe.GetPointer<Shakeable>(entity),
                    Stats = f.Unsafe.GetPointer<Stats>(entity)
                };

                Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
                Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

                animEvent.End(f, default, ref filter, input, 0);
                list.RemoveAt(0);
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