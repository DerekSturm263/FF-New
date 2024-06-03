using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum.Custom.Animator
{
  [Serializable]
  public unsafe class AnimatorState
  {
    public int id;
    public string name;
    public bool isAny;
    public bool isDefault;
    public FP cycleOffset = FP._0;
    public FP speed = FP._1;
    [NonSerialized]
    public AnimatorMotion motion;

    public AnimatorTransition[] transitions;

    public List<SerializableMotion> serialisedMotions;

    // AssetRef for custom baking. Insert any asset you want here from the Unity baking
    public AssetRefQuantumAnimationEvent StateAsset;

    /// <summary>
    /// Progress the state machine state by a frame
    /// </summary>
    public void Update(Frame f, EntityRef* e, CustomAnimator* a, CustomAnimatorGraph graph, AnimatorLayer layer, FP deltaTime)
    {
      if (!isAny)
      {
        if ((motion == null || motion.isEmpty) && !isDefault)
        {
          a->current_state_id = 0;
          a->from_state_id = 0;
          a->to_state_id = 0;
          return;
        }

        if (motion != null && !motion.isEmpty)
        {
          motion.CalculateWeights(f, a, id);
          FP deltaTimeSpeed = deltaTime * FPMath.Abs(speed);

          //advance time - current state
          if (id == a->current_state_id && a->to_state_id == 0)
          {
            var length = motion.CalculateLength(f, a, FP._1, this);
            if (length == FP._0)
            {
              return;
            }

            FP sampleTime = a->normalized_time * length;
            FP currentTime = sampleTime + deltaTimeSpeed;
            FP lastTime = sampleTime;

            if (!motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = length;//clamp
              if (length < lastTime) lastTime = currentTime - deltaTimeSpeed;//clamp
            }
            if (motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = currentTime % length;
              lastTime = currentTime - deltaTimeSpeed;
            }

            FP normalizedTime = currentTime / length;

            if (motion.looped)
            {
              normalizedTime = normalizedTime % FP._1;
            }
            else
            {
              normalizedTime = FPMath.Clamp(normalizedTime, FP._0, FP._1);
            }

            a->time = currentTime;
            a->last_time = lastTime;
            a->normalized_time = normalizedTime;
            a->length = length;

            f.Signals.OnAnimatorStateUpdate(*e, a);

          }

          //advance time - transistion state
          if (a->from_state_id == id)
          {
            var length = motion.CalculateLength(f, a, FP._1, this);
            if (length == FP._0) //lengthless motion - ignore
              return;

            FP sampleTime = a->from_state_normalized_time * length;
            FP lastTime = sampleTime;
            FP currentTime = sampleTime + deltaTimeSpeed;

            if (!motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = length;//clamp
              if (length < lastTime) lastTime = currentTime - deltaTimeSpeed;//clamp
            }
            if (motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = currentTime % length;
              lastTime = currentTime - deltaTimeSpeed;
            }

            FP normalisedTime = currentTime / length;

            if (motion.looped)
            {
              normalisedTime = normalisedTime % FP._1;
            }
            else
            {
              normalisedTime = FPMath.Clamp(normalisedTime, FP._0, FP._1);
            }

            a->from_state_time = currentTime;
            a->from_state_last_time = lastTime;
            a->from_state_normalized_time = normalisedTime;
            a->from_length = length;


          }

          if (a->to_state_id == id)
          {
            var length = motion.CalculateLength(f, a, FP._1, this);
            if (length == FP._0) //lengthless motion - ignore
              return;

            FP sampleTime = a->to_state_normalized_time * length;
            FP lastTime = sampleTime;
            FP currentTime = sampleTime + deltaTimeSpeed;

            if (!motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = length;//clamp
              if (length < lastTime) lastTime = currentTime - deltaTimeSpeed;//clamp
            }
            if (motion.looped && length + deltaTimeSpeed < currentTime)
            {
              currentTime = currentTime % length;
              lastTime = currentTime - deltaTimeSpeed;
            }

            FP normalisedTime = currentTime / length;

            if (motion.looped)
            {
              normalisedTime = normalisedTime % FP._1;
            }
            else
            {
              normalisedTime = FPMath.Clamp(normalisedTime, FP._0, FP._1);
            }

            a->to_state_time = currentTime;
            a->to_state_last_time = lastTime;
            a->to_state_normalized_time = normalisedTime;
            a->to_length = length;
                        
            f.Signals.OnAnimatorStateUpdate(*e, a);
          }
        }
      }

      for (Int32 i = 0; i < transitions.Length; i++)
      {
        transitions[i].Update(f, e, a, graph, this, deltaTime);
      }
    }

    public FP GetLength(Frame f, CustomAnimator* a)
    {
      if (motion != null && !motion.isEmpty)
        return motion.CalculateLength(f, a, FP._1, this);
      return FP._0;
    }

    /// <summary>
    /// Generate the blend list
    /// This is a list of all the animations used in the current state machine frame
    /// The output will be a list of animations and weights that can be used to pose an animation
    /// </summary>
    /// <param name="list">The list to build into</param>
    public void GenerateBlendList(Frame f, CustomAnimator* a, CustomAnimatorGraph graph, AnimatorLayer layer, List<AnimatorRuntimeBlendData> list)
    {

      if (!isAny)
      {
        if (motion == null || motion.isEmpty && !isDefault)
        {
          return;
        }

        motion.CalculateWeights(f, a, id);

        var length = motion.CalculateLength(f, a, FP._1, this);
        if (length == FP._0)
        {
          return;
        }

        motion.GenerateBlendList(f, a, layer, this, FP._1, list);
      }
    }

    /// <summary>
    /// Get a motion from within a blend tree by the index
    /// </summary>
    /// <returns></returns>
    public AnimatorMotion GetMotion(int treeIndex, List<AnimatorMotion> proccessList)
    {
      if (motion != null)
      {
        proccessList.Add(motion);
      }

      while (proccessList.Count > 0)
      {
        AnimatorMotion current = proccessList[0];
        proccessList.RemoveAt(0);

        if (current.treeIndex == treeIndex)
        {
          return current;
        }

        if (current.isTree)
        {
          AnimatorBlendTree tree = current as AnimatorBlendTree;

          if (tree != null)
          {
            proccessList.AddRange(tree.motions);
          }
        }
      }

      return null;
    }
  }
}