﻿using Photon.Deterministic;

namespace Quantum
{
    public unsafe sealed class BurstState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Burst;

        public override (States, StatesFlag) GetStateInfo() => (States.Burst, StatesFlag.Burst);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) =>
        [
            new(destination: States.Emote, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Interact, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Jump, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Primary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Secondary, transitionTime: settings.InputCheckTime, overrideExit: false, overrideEnter: false),
            new(destination: States.Sub, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Ultimate, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Crouch, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.LookUp, transitionTime: 0, overrideExit: false, overrideEnter: false),
            new(destination: States.Default, transitionTime: 0, overrideExit: false, overrideEnter: false)
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => settings.BurstTime;

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                filter.Stats->CurrentStats.Energy >= settings.BurstCost;
        }

        public override void FinishEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States previousState)
        {
            base.FinishEnter(f, ref filter, input, settings, previousState);

            filter.PhysicsBody->GravityScale = 0;
            filter.PhysicsBody->Velocity = FPVector2.Zero;
            filter.CharacterController->Velocity = 0;

            StatsSystem.ModifyEnergy(f, filter.Entity, filter.Stats, -settings.BurstCost);
        }

        public override void FinishExit(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, States nextState)
        {
            filter.PhysicsBody->GravityScale = 1;

            base.FinishExit(f, ref filter, input, settings, nextState);
        }
    }
}
