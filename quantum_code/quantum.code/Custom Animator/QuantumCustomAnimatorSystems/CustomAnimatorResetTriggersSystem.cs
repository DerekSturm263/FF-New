using Quantum.Custom.Animator;

namespace Quantum
{
  public unsafe class CustomAnimatorResetTriggersSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      var filter = f.Filter<CustomAnimator>();
      while (filter.NextUnsafe(out _, out var animator))
      {
        if (animator->animatorGraph.Id.Equals(default))
        {
          continue;
        }

        CustomAnimatorGraph animatorGraph = f.FindAsset<CustomAnimatorGraph>(animator->animatorGraph.Id);
        foreach (var variable in animatorGraph.variables)
        {
          if (variable.type != AnimatorVariable.VariableType.Trigger)
          {
            continue;
          }

          if (CustomAnimator.IsTriggerActive(f, animator, variable.index))
          {
            CustomAnimator.ResetTrigger(f, animator, variable.index);
          }
        }
      }
    }
  }
}
