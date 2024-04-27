using Photon.Deterministic;
using System;
using System.Collections.Generic;

namespace Quantum.Custom.Animator
{
  [Serializable]
  public unsafe class AnimatorLayer {
    public int id;
    public string name;
    public AnimatorState[] states;

    public void Update(Frame f, CustomAnimatorGraph graph, EntityRef* entity, CustomAnimator* a, FP deltaTime) {
      for (int i = 0; i < states.Length; i++) {
        var state = states[i];
        if (IsStateActive(a, state)) {
          state.Update(f, entity, a, graph, this, deltaTime);
        } else if (states[i].isDefault && a->current_state_id == 0) {
          a->current_state_id = states[i].id;
          state.Update(f, entity, a, graph, this, deltaTime);
        }
      }

      if (a->to_state_id != 0)//transition occuring
      {
        a->transition_time += deltaTime;//advance transition time
        if (a->transition_time >= a->transition_duration)//on completion 
        {
          a->current_state_id = a->to_state_id;
          a->time = a->to_state_time;
          a->last_time = a->to_state_last_time;
          a->normalized_time = FPMath.Clamp(a->to_state_time / a->to_length, FP._0, FP._1);
          //reset transition state
          a->from_state_id = 0;
          a->from_state_time = FP._0;
          a->from_state_last_time = FP._0;
          a->from_state_normalized_time = FP._0;
          a->from_length = FP._0;

          a->to_state_id = 0;
          a->to_state_time = FP._0;
          a->to_state_last_time = FP._0;
          a->to_state_normalized_time = FP._0;
          a->to_length = FP._0;

          a->transition_index = 0;
          a->transition_time = FP._0;
          a->transition_duration = FP._0;
        }
      }
    }

    public void GenerateBlendList(Frame f, CustomAnimatorGraph graph, CustomAnimator* a, List<AnimatorRuntimeBlendData> list) {
      for (int i = 0; i < states.Length; i++) {
        var state = states[i];

        if (IsStateActive(a, state)) {
          state.GenerateBlendList(f, a, graph, this, list);
        } else if (states[i].isDefault && a->current_state_id == 0) {
          a->current_state_id = states[i].id;
          state.GenerateBlendList(f, a, graph, this, list);
        }
      }
    }

    bool IsStateActive(CustomAnimator* a, AnimatorState state) {
      var isCurrentState = a->current_state_id == state.id;
      var isTransitionState = a->to_state_id == state.id || a->from_state_id == state.id;
      var isTransitioning = a->to_state_id != 0;

      if (isCurrentState && !isTransitioning || isTransitionState && isTransitioning || state.isAny) {
        return true;
      }

      return false;
    }
  }
}
