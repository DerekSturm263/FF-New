namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class ContinueAnimationEvent : FrameEvent
    {
        public AssetRefPlayerState Default;
        public Input.Buttons Button;

        public override void Begin(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Continuing animation!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                characterController->PressedButton = false;
        }

        public override void Update(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame, int elapsedFrames)
        {
            if (!f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
                return;

            // TODO: FIX TO WORK WITH BOTS
            Behavior behavior = f.FindAsset<Behavior>(characterController->Behavior.Id);
            Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

            if (characterController->WasPressedThisFrame(input, Button))
                characterController->PressedButton = true;
        }

        public override void End(Frame f, QuantumAnimationEvent parent, EntityRef entity, int frame)
        {
            Log.Debug("Cleaning up animation continuing!");

            if (f.Unsafe.TryGetPointer(entity, out CharacterController* characterController))
            {
                CharacterControllerSystem.Filter filter = new()
                {
                    Entity = entity,

                    CharacterController = f.Unsafe.GetPointer<CharacterController>(entity),
                    Transform = f.Unsafe.GetPointer<Transform2D>(entity),
                    PhysicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(entity),
                    CustomAnimator = f.Unsafe.GetPointer<CustomAnimator>(entity),
                    Stats = f.Unsafe.GetPointer<Stats>(entity),
                    PlayerStats = f.Unsafe.GetPointer<PlayerStats>(entity),
                    Shakeable = f.Unsafe.GetPointer<Shakeable>(entity)
                };

                // TODO: FIX TO WORK WITH BOTS
                Behavior behavior = f.FindAsset<Behavior>(characterController->Behavior.Id);
                MovementSettings settings = f.FindAsset<MovementSettings>(filter.CharacterController->Settings.Id);

                Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(entity).Player) : behavior.GetInput(f, default);

                if (!characterController->PressedButton)
                {
                    CharacterControllerSystem.StateMachine.ForceTransition(f, ref filter, input, settings, Default, 0);
                    characterController->PossibleStates = (StatesFlag)((int)StatesFlag.KnockedOver * 2 - 1);
                }

                characterController->PressedButton = false;
            }
        }
    }
}
