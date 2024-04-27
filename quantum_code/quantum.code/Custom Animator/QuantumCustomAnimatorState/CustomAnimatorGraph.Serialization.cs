using Photon.Deterministic;
using Quantum.Collections;
using System;
using System.Collections.Generic;

namespace Quantum
{
    public partial class CustomAnimatorGraph {
    public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator) {
      Deserialize(this);
    }

    public static void Serialize(CustomAnimatorGraph animatorGraph) {
      int layerCount = animatorGraph.layers.Length;
      for (int l = 0; l < layerCount; l++) {
        Custom.Animator.AnimatorLayer layer = animatorGraph.layers[l];
        if (layer.states == null) layer.states = new Custom.Animator.AnimatorState[0];
        int stateCount = layer.states.Length;
        for (int s = 0; s < stateCount; s++) {
          Custom.Animator.AnimatorState state = layer.states[s];
          if (state.serialisedMotions == null)
            state.serialisedMotions = new List<Custom.Animator.SerializableMotion>();
          state.serialisedMotions.Clear();
          SerializeObject(state);
        }
      }
    }

    public static void Deserialize(CustomAnimatorGraph animatorGraph) {
      int layerCount = animatorGraph.layers.Length;
      for (int l = 0; l < layerCount; l++) {
        Custom.Animator.AnimatorLayer layer = animatorGraph.layers[l];
        int stateCount = layer.states.Length;
        for (int s = 0; s < stateCount; s++) {
          Custom.Animator.AnimatorState state = layer.states[s];
          if (state.serialisedMotions.Count > 0) {
            state.motion = ReadNodeFromSerializedNodes(state, 0);
          }
        }
      }
    }

    private static void SerializeObject(Custom.Animator.AnimatorState state, Custom.Animator.AnimatorMotion mo = null) {
      Custom.Animator.SerializableMotion serialisedBo = new Custom.Animator.SerializableMotion();

      if (mo == null)//initial blend object will be patched in on first call
        mo = state.motion;

      if (mo is Custom.Animator.AnimatorClip) {
        Custom.Animator.AnimatorClip anim = mo as Custom.Animator.AnimatorClip;
        serialisedBo.isAnimation = true;
        serialisedBo.name = anim.data.clipName;
        serialisedBo.animatorData = anim.data;
        serialisedBo.childCount = 0;
        serialisedBo.indexOfFirstChild = state.serialisedMotions.Count + 1;

        state.serialisedMotions.Add(serialisedBo);
      }

      if (mo is Custom.Animator.AnimatorBlendTree) {
        Custom.Animator.AnimatorBlendTree blend = mo as Custom.Animator.AnimatorBlendTree;
        serialisedBo.isAnimation = false;
        serialisedBo.name = blend.name;//string.Format("Tree of {0}", blend.motionCount);
        serialisedBo.positions = blend.positions;
        serialisedBo.blendParameterIndex = blend.blendParameterIndex;
        serialisedBo.blendParameterIndexY = blend.blendParameterIndexY;
        serialisedBo.weightTable = SerializeWeightTable(blend.weightTable);
        serialisedBo.resolution = blend.resolution;

        serialisedBo.childCount = blend.motionCount;
        serialisedBo.indexOfFirstChild = state.serialisedMotions.Count + 1;

        state.serialisedMotions.Add(serialisedBo);
        foreach (var child in blend.motions)
          SerializeObject(state, child);
      }
    }

    private static Custom.Animator.AnimatorMotion ReadNodeFromSerializedNodes(Custom.Animator.AnimatorState state, int index) {
      Custom.Animator.SerializableMotion serialisedBo = state.serialisedMotions[index];
      List<Custom.Animator.AnimatorMotion> children = new List<Custom.Animator.AnimatorMotion>();
      for (int i = 0; i < serialisedBo.childCount; i++)
        children.Add(ReadNodeFromSerializedNodes(state, serialisedBo.indexOfFirstChild + i));

      if (serialisedBo.isAnimation) {
        Custom.Animator.AnimatorClip anim = new Custom.Animator.AnimatorClip();
        anim.name = serialisedBo.name;
        anim.data = serialisedBo.animatorData;
        anim.treeIndex = index;
        return anim;
      } else {
        Custom.Animator.AnimatorBlendTree blend = new Custom.Animator.AnimatorBlendTree();
        blend.name = serialisedBo.name;
        blend.positions = serialisedBo.positions;
        blend.blendParameterIndex = serialisedBo.blendParameterIndex;
        blend.blendParameterIndexY = serialisedBo.blendParameterIndexY;
        blend.resolution = serialisedBo.resolution;
        blend.weightTable = DeserializeWeightTable(serialisedBo.weightTable);
        blend.motions = children.ToArray();
        blend.motionCount = children.Count;
        //blend.weights = new FP[blend.motionCount];
        blend.treeIndex = index;
        return blend;
      }
    }

        private static FP[,][] DeserializeWeightTable(Custom.Animator.SerializableWeightDimentionX table) {
      int xLength = table.data.Length;
      if (xLength == 0) return new FP[0, 0][];
      int yLength = table.data[0].data.Length;
      FP[,][] output = new FP[xLength, yLength][];
      for (int x = 0; x < xLength; x++) {
        for (int y = 0; y < yLength; y++) {
          output[x, y] = table.data[x].data[y].data;
        }
      }
      return output;
    }

    private static Custom.Animator.SerializableWeightDimentionX SerializeWeightTable(FP[,][] table) {
      int xLength = table.GetLength(0);
      int yLength = table.GetLength(1);
      if (xLength == 0) return new Custom.Animator.SerializableWeightDimentionX();
      Custom.Animator.SerializableWeightDimentionX output = new Custom.Animator.SerializableWeightDimentionX();
      output.data = new Custom.Animator.SerializableWeightDimentionY[xLength];
      for (int x = 0; x < xLength; x++) {
        output.data[x].data = new Custom.Animator.SerializableWeightDimentionZ[yLength];
        for (int y = 0; y < yLength; y++) {
          output.data[x].data[y].data = table[x, y];
        }
      }
      return output;
    }
  }
}
