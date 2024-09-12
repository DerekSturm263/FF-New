namespace Quantum
{
    [System.Serializable]
    public unsafe partial class FriedStatusEffect : StatusEffect
    {
        public override void OnApply(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.CharacterController->CanInput = false;
            filter.Stats->StatusEffectTimeLeft = (filter.Stats->StatusEffectTimeLeft * filter.Stats->StatusEffectMultiplier).AsInt;
        }

        public override void OnRemove(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            filter.CharacterController->CanInput = true;
        }
    }
}
