using System;
using System.Diagnostics;
using Photon.Deterministic;

namespace Quantum.Custom.Animator
{

  [Serializable]
  public unsafe class AnimatorTransition
  {

    public int index;
    public string name;

    /// <summary>
    /// The duration of the transition.
    /// </summary>
    public FP duration;

    /// <summary>
    /// The time at which the destination state will start.
    /// </summary>
    public FP offset;

    /// <summary>
    /// If AnimatorStateTransition.has_exit_time is true, exit_time represents the exact time at which the transition can take effect.
    /// This is represented in normalized time, so for example an exit time of 0.75 means that on the first frame where 75% of the animation has played, the Exit Time condition will be true. On the next frame, the condition will be false.
    /// For looped animations, transitions with exit times smaller than 1 will be evaluated every loop, so you can use this to time your transition with the proper timing in the animation, every loop.
    /// Transitions with exit times greater than one will be evaluated only once, so they can be used to exit at a specific time, after a fixed number of loops. For example, a transition with an exit time of 3.5 will be evaluated once, after three and a half loops.</summary>
    public FP exitTime;

    /// <summary>
    /// When active the transition will have an exit time condition.
    /// </summary>
    public bool hasExitTime;

    /// <summary>
    /// Allow the transition to occur if the current state is the same as the next state
    /// </summary>
    public bool canTransitionToSelf;

    /// <summary>
    /// AnimatorCondition conditions that need to be met for a transition to happen.
    /// </summary>
    public AnimatorCondition[] conditions;

    public int destinationStateId;
    public string destinationStateName;


    public void Update(Frame f, EntityRef* entity, CustomAnimator* a, CustomAnimatorGraph graph, AnimatorState state, FP deltaTime)
    {
      if (duration == FP._0 && !state.isAny)
      {
        //Log.Warn(string.Format("Transistion {0} has a duration of 0", name));
      }
      bool noCurrentTransition = a->to_state_id == 0;
      bool selfConditional = a->current_state_id != destinationStateId || canTransitionToSelf;

      if (hasExitTime == false && conditions.Length == 0)
        return;

      if (noCurrentTransition && selfConditional)
      {
        if (!hasExitTime || a->time > exitTime)
        {
          bool conditionsMet = true;
          for (int c = 0; c < conditions.Length; c++)
          {
            if (!conditions[c].Check(f, a, graph))
            {
              conditionsMet = false;
              break;
            }
          }

          if (conditionsMet)
          {
            //fill in a transition state
            f.Signals.OnAnimatorStateExit(*entity, a);

            a->transition_time = FP._0;
            a->transition_duration = duration;
            a->transition_index = index;

            a->from_state_id = a->current_state_id;
            a->from_state_time = a->time;
            a->from_state_last_time = a->last_time;
            a->from_state_normalized_time = a->normalized_time;
            a->from_length = a->length;

            a->to_state_id = destinationStateId;
            a->to_state_time = offset;
            a->to_state_last_time = FPMath.Max(offset-deltaTime, FP._0);

            // If AnimatorState.Update run the code for s, the weights are not initialized and we get a divide by zero exception.
            var s = graph.GetState(a->to_state_id);
            if (s.GetLength(f, a) == 0)
            {
              s.motion.CalculateWeights(f, a, a->to_state_id);
            }

            a->to_length = s.GetLength(f, a);
            a->to_state_normalized_time = FPMath.Clamp(a->to_state_time / a->to_length, FP._0, FP._1);

                        //fill in a transition state
                        f.Signals.OnAnimatorStateEnter(*entity, a);
                    }
        }
      }
    }
  }
}
