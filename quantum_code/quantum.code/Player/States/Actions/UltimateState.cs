namespace Quantum
{
    public unsafe sealed class UltimateState : ActionState
    {
        protected override Input.Buttons GetInput() => Input.Buttons.Ultimate;

        public override (States, StatesFlag) GetStateInfo() => (States.Ultimate, 0);
        public override EntranceType GetEntranceType() => EntranceType.Grounded | EntranceType.Aerial;

        public override TransitionInfo[] GetTransitions() =>
        [
            new() { Destination = States.Burst },
            new() { Destination = States.Dodge },
            new() { Destination = States.Emote },
            new() { Destination = States.Interact },
            new() { Destination = States.Jump },
            new() { Destination = States.Primary },
            new() { Destination = States.Secondary },
            new() { Destination = States.Sub },
            new() { Destination = States.Block },
            new() { Destination = States.Crouch },
            new() { Destination = States.LookUp },
            new() { Destination = States.Default }
        ];

        protected override int StateTime(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);

                if (animEvent is not null && animEvent.AnimID != 0)
                    return CustomAnimator.GetStateLength(f, filter.CustomAnimator, animEvent.AnimID);
            }

            return 0;
        }

        protected override bool CanEnter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return base.CanEnter(f, ref filter, input, settings) &&
                f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate _) &&
                filter.Stats->CurrentStats.Energy >= f.Global->CurrentMatch.Ruleset.Players.MaxEnergy &&
                filter.CharacterController->UltimateTime == 0;
        }

        public override void Enter(Frame f, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Enter(f, ref filter, input, settings);

            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Ultimate.Id, out Ultimate ultimate))
            {
                QuantumAnimationEvent animEvent = f.FindAsset<QuantumAnimationEvent>(ultimate.Move.Id);
                if (animEvent is not null)
                    CustomAnimator.SetCurrentState(f, filter.CustomAnimator, animEvent.AnimID);

                ultimate.OnBegin(f, filter.Entity);
                
                filter.CharacterController->UltimateTime = ultimate.Length;
            }
        }
    }
}