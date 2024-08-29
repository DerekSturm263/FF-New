namespace Quantum
{
    public unsafe class CharacterControllerButtonResetSystem : SystemMainThreadFilter<CharacterControllerSystem.Filter>
    {
        public override void Update(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            Behavior behavior = f.FindAsset<Behavior>(filter.CharacterController->Behavior.Id);
            Input input = behavior.IsControllable ? *f.GetPlayerInput(f.Get<PlayerLink>(filter.Entity).Player) : behavior.GetInput(f, filter);

            filter.CharacterController->LastFrame = input;
        }
    }
}
