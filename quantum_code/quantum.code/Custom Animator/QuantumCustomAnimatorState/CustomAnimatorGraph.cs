using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Custom.Animator;
using CA = Quantum.Custom.Animator;

namespace Quantum
{
  /// <summary>
  /// Extension class for the CustomAnimator asset
  /// If you are using Quantum 1.2.X, please make sure to only use the Animator classes/structs
  /// which are under the namespace Quantum.Custom.Animator, otherwise you will be using the
  /// original Animator classes
  /// </summary>
  public unsafe partial class CustomAnimatorGraph
  {
    public CA.AnimatorLayer[] layers;
    public CA.AnimatorVariable[] variables;

    public bool rootMotion = false;
    public bool rootMotionAppliesPhysics = false;

    public void Initialise(Frame f, CustomAnimator* a)
    {
      a->animatorGraph = this;
      a->speed = FP._1;
      a->current_state_id = 0;
      a->to_state_id = 0;
      a->transition_time = FP._0;
      a->transition_duration = FP._0;

      var blendTreeWeights = f.AllocateDictionary<int, BlendTreeWeights>();
      for (int i = 0; i < layers.Length; i++)
      {
        var layer = layers[i];
        for (int j = 0; j < layer.states.Length; j++)
        {
          var state = layer.states[j];
          var weightsList = f.AllocateList<FP>();
          if(state.motion is AnimatorBlendTree tree)
          {
            for (int k = 0; k < tree.motionCount; k++)
            {
              weightsList.Add(0);
            }
          }
          blendTreeWeights.Add(state.id, new BlendTreeWeights { Values = weightsList });
        }
      }
      a->BlendTreeWeights = blendTreeWeights;

      if (a->AnimatorVariables.Ptr != default)
      {
        f.FreeList(a->AnimatorVariables);
      }

      var variablesList = f.AllocateList<CustomAnimatorRuntimeVariable>(variables.Length);

      // set variable defaults
      for (Int32 v = 0; v < variables.Length; v++)
      {
        CustomAnimatorRuntimeVariable newParameter = new CustomAnimatorRuntimeVariable();
        switch (variables[v].type)
        {
          case CA.AnimatorVariable.VariableType.FP:
            *newParameter.FPValue = variables[v].defaultFp;
            break;

          case CA.AnimatorVariable.VariableType.Int:
            *newParameter.IntegerValue = variables[v].defaultInt;
            break;

          case CA.AnimatorVariable.VariableType.Bool:
            *newParameter.BooleanValue = variables[v].defaultBool;
            break;

          case CA.AnimatorVariable.VariableType.Trigger:
            *newParameter.BooleanValue = variables[v].defaultBool;
            break;
        }

        variablesList.Add(newParameter);
      }

      a->AnimatorVariables = variablesList;
    }

    /// <summary>
    /// Updates the state machine graph
    /// </summary>
    public void UpdateGraphState(Frame f, EntityRef* entity, CustomAnimator* a, FP deltaTime)
    {
      for (Int32 i = 0; i < layers.Length; i++)
      {
        layers[i].Update(f, this, entity, a, deltaTime);
      }
    }

    /// <summary>
    /// Cross fade from the current state to a specific state in the state machine
    /// This will override the current transistions until this cross fade is complete
    /// Once the crossfade completes, the state machine will continue normal operation from the destination state of the crossfade
    /// </summary>
    /// <param name="a"></param>
    /// <param name="stateNameHash"></param>
    /// <param name="transistionDuration"></param>
    public void CrossFade(CustomAnimator* a, int stateNameHash, FP transistionDuration)
    {
      if (a->to_state_id == 0)
      {
        a->last_time = FP._0;
        a->time = FP._0;
        a->normalized_time = FP._0;

        a->to_state_id = stateNameHash;
        a->transition_time = FP._0;
        a->transition_duration = transistionDuration;
      }
    }

    /// <summary>
    /// Generate a list of weighted animations to use in posing an animation
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public void GenerateBlendList(Frame f, CustomAnimator* a, List<CA.AnimatorRuntimeBlendData> output)
    {
      for (Int32 i = 0; i < layers.Length; i++)
      {
        layers[i].GenerateBlendList(f, this, a, output);
      }

      // normalise
      var blendCount = output.Count;
      var totalWeight = FP._0;

      for (int b = 0; b < blendCount; b++)
      {
        totalWeight += output[b].weight;
      }

      if (totalWeight == FP._0) totalWeight = FP._1;

      for (int b = 0; b < blendCount; b++)
      {
        CA.AnimatorRuntimeBlendData blend = output[b];
        blend.weight /= totalWeight;//normalise
        output[b] = blend;
      }
    }

    public CA.AnimatorFrame CalculateRootMotion(Frame f, CustomAnimator* a, List<CA.AnimatorRuntimeBlendData> blendList, List<CA.AnimatorMotion> motionList)
    {
      GenerateBlendList(f, a, blendList);

      int blendSize = blendList.Count;
      CA.AnimatorFrame output = new CA.AnimatorFrame();

      for (Int32 i = 0; i < blendSize; i++)
      {
        var b = blendList[i];
        if (b.stateId == 0)
        {
          continue;
        }

        var state = GetState(b.stateId);
        if (state == null)
        {
          continue;
        }

        var motion = state.GetMotion(b.animationIndex, motionList);
        if (motion != null)
        {
          if (motion is CA.AnimatorClip clip)
          {
            output += clip.data.CalculateDelta(b.lastTime, b.currentTime) * b.weight;
          }
        }
      }

      return output;
    }

    public CA.AnimatorState GetState(int stateId)
    {
      for (Int32 l = 0; l < layers.Length; l++)
      {
        for (Int32 s = 0; s < layers[l].states.Length; s++)
        {
          if (layers[l].states[s].id == stateId)
          {
            return layers[l].states[s];
          }
        }
      }
      return null;
    }

    public Int32 VariableIndex(string name)
    {
      for (Int32 v = 0; v < variables.Length; v++)
      {
        if (variables[v].name == name)
        {
          return v;
        }
      }

      return -1;
    }
  }
}