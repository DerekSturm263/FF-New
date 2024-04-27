using System;
using System.Collections.Generic;
using System.Diagnostics;
using Photon.Deterministic;
using Quantum.Collections;

namespace Quantum.Custom.Animator
{
  public unsafe interface IAnimatorMotion
  {
    string name { get; set; }
    bool isEmpty { get; }
    bool isTree { get; }
    bool looped { get; }
    int treeIndex { get; set; }
    void GenerateBlendList(Frame f, CustomAnimator* a, AnimatorLayer layer, AnimatorState state, FP weight, List<AnimatorRuntimeBlendData> list);
    FP CalculateLength(Frame f, CustomAnimator* a, FP weight, AnimatorState state);
    void CalculateWeights(Frame f, CustomAnimator* a, int stateId);
  }

  public abstract unsafe class AnimatorMotion : IAnimatorMotion
  {
    public virtual string name { get; set; }
    public virtual bool isEmpty { get { return false; } }
    public virtual bool isTree { get { return false; } }
    public virtual bool looped { get { return false; } }
    public virtual int treeIndex { get { return -1; } set { } }
    public virtual void GenerateBlendList(Frame f, CustomAnimator* a, AnimatorLayer layer, AnimatorState state, FP weight, List<AnimatorRuntimeBlendData> list) { }
    public virtual FP CalculateLength(Frame f, CustomAnimator* a, FP weight, AnimatorState state) { return FP._0; }
    public virtual void CalculateWeights(Frame f, CustomAnimator* a, int stateId) { }
  }

  public unsafe class AnimatorClip : AnimatorMotion
  {
    public AnimatorData data;
    public int _treeIndex;

    public override bool isEmpty { get { return data.frameCount == 0; } }

    public override bool isTree { get { return false; } }

    public override bool looped { get { return data.looped; } }

    public string clipName { get { return data != null ? data.clipName : ""; } }

    public override int treeIndex
    {
      get { return _treeIndex; }
      set { _treeIndex = value; }
    }

    public override void GenerateBlendList(Frame f, CustomAnimator* a, AnimatorLayer layer, AnimatorState state, FP weight, List<AnimatorRuntimeBlendData> list)
    {
      FP time = FP._0;
      FP lastTime = FP._0;
      FP normalizedTime = FP._0;
      FP calculatedLength = FP._0;
      FP clipWeight = weight;
      FP length = data.length;

      if (state.id == a->current_state_id)
      {
        time = a->time;
        lastTime = a->last_time;
        normalizedTime = a->normalized_time;
        calculatedLength = a->length;
      }

      if (state.id == a->from_state_id)
      {
        time = a->from_state_time;
        lastTime = a->from_state_last_time;
        normalizedTime = a->from_state_normalized_time;
        calculatedLength = a->from_length;
      }

      if (state.id == a->to_state_id)
      {
        time = a->to_state_time;
        lastTime = a->to_state_last_time;
        normalizedTime = a->to_state_normalized_time;
        calculatedLength = a->to_length;
      }

      if (a->to_state_id != 0)
      {// transition is happening
        FP transitionWeight = FPMath.Clamp(a->transition_time / a->transition_duration, FP._0, FP._1);
        if (state.id == a->from_state_id) transitionWeight = FP._1 - transitionWeight;
        clipWeight = weight * transitionWeight;
      }

      AnimatorRuntimeBlendData blendData = new AnimatorRuntimeBlendData(layer, state, treeIndex, lastTime, time, normalizedTime, clipWeight, length, calculatedLength);
      list.Add(blendData);
    }

    public override FP CalculateLength(Frame f, CustomAnimator* a, FP weight, AnimatorState state)
    {
      if (data == null)
        return 0;
      else
        return data.length * weight;
    }

    public override void CalculateWeights(Frame f, CustomAnimator* a, int stateId)
    {
      //not implemented
    }
  }

  public unsafe class AnimatorBlendTree : AnimatorMotion
  {

    public AnimatorMotion[] motions;
    public FPVector2[] positions;
    //private FP[] weights;
    public int resolution;
    public FP[,][] weightTable;
    public int motionCount;
    public int _treeIndex;

    public int blendParameterIndex = -1;
    public int blendParameterIndexY = -1;
        
    public override bool isEmpty { get { return motionCount == 0; } }

    public override bool isTree { get { return true; } }

    public override bool looped { get { return true; } }

    public override int treeIndex
    {
      get { return _treeIndex; }
      set { _treeIndex = value; }
    }

    public override void GenerateBlendList(Frame f, CustomAnimator* a, AnimatorLayer layer, AnimatorState state, FP weight, List<AnimatorRuntimeBlendData> list)
    {
      FP zf = FP._0_05;
      var weights = CustomAnimator.GetStateWeights(f, a, state.id);
      for (int i = 0; i < motionCount; i++)
        if (weights[i] > zf)
          motions[i].GenerateBlendList(f, a, layer, state, weights[i], list);
    }

    public override FP CalculateLength(Frame f, CustomAnimator* a, FP weight, AnimatorState state)
    {
      FP output = FP._0;
      FP zf = FP._0_05;
      var weights = CustomAnimator.GetStateWeights(f, a, state.id);
      for (int i = 0; i < motionCount; i++)
        if (motions[i] != null && weights[i] > zf)
          output += motions[i].CalculateLength(f, a, weights[i], state);
      return output;
    }

    public bool Is2D()
    {
      return blendParameterIndex != blendParameterIndexY;
    }

    public override void CalculateWeights(Frame f, CustomAnimator* a, int stateId)
    {
      var weights = CustomAnimator.GetStateWeights(f, a, stateId);

      //ClearWeights();

      if (motionCount == 0)
      {
        Log.Warn("No motions to blend");
        return;
      }

      if (motionCount == 1)
      {
        *weights.GetPointer(0) = FP._1;
        return;
      }

      FP totalWeight = FP._0;
      if (Is2D())
      { // 2d blend calculation
        //ASUMPTION - blend variables are between -1 and 1
        FP blendParameterX = FPMath.Clamp(*CustomAnimator.Variable(f, a, blendParameterIndex)->FPValue, -FP._1, FP._1);
        FP blendParameterY = FPMath.Clamp(*CustomAnimator.Variable(f, a, blendParameterIndexY)->FPValue, -FP._1, FP._1);

        blendParameterX = blendParameterX / FP._2 + FP._0_50;
        blendParameterY = blendParameterY / FP._2 + FP._0_50;

        FP res = resolution - 2;
        FP blendIndexXLerp = blendParameterX * res;
        FP blendIndexYLerp = blendParameterY * res;

        //TODO - replace with FP.FloorToInt/CeilToInt when available
        int blendIndexXa = FPMath.Floor(blendIndexXLerp).AsInt;
        int blendIndexXb = blendIndexXa + 1;
        int blendIndexYa = FPMath.Floor(blendIndexYLerp).AsInt;
        int blendIndexYb = blendIndexYa + 1;

        blendIndexXLerp = blendIndexXLerp - blendIndexXa;
        blendIndexYLerp = blendIndexYLerp - blendIndexYa;

        for (int z = 0; z < motionCount; z++)
        {
          FP weightA = weightTable[blendIndexXa, blendIndexYa][z];
          FP weightB = weightTable[blendIndexXb, blendIndexYa][z];
          FP weightC = weightTable[blendIndexXa, blendIndexYb][z];
          FP weightD = weightTable[blendIndexXb, blendIndexYb][z];

          FP lerpa = FPMath.Lerp(weightA, weightB, blendIndexXLerp);
          FP lerpb = FPMath.Lerp(weightA, weightC, blendIndexYLerp);
          FP lerpc = FPMath.Lerp(weightB, weightD, blendIndexYLerp);
          FP lerpd = FPMath.Lerp(weightC, weightD, blendIndexXLerp);
          *weights.GetPointer(z) = (lerpa + lerpb + lerpc + lerpd) / 4;
          totalWeight += weights[z];
        }
      }
      else
      {
        FP blendParameter = FPMath.Clamp(*CustomAnimator.Variable(f, a, blendParameterIndex)->FPValue / FP._2 + FP._0_50, -1, 1);
        FP lerp = blendParameter * (resolution - 2);
        int indexA = FPMath.CeilToInt(lerp);
        int indexB = indexA + 1;
        lerp = lerp - indexA;

        for (int z = 0; z < motionCount; z++)
        {
          FP weightA = weightTable[indexA, 0][z];
          FP weightB = weightTable[indexB, 0][z];
          *weights.GetPointer(z) = FPMath.Lerp(weightA, weightB, lerp);
          totalWeight += weights[z];
        }
      }
    }

    //private void ClearWeights()
    //{
    //  if (weights.Length != motionCount)
    //    weights = new FP[motionCount];
    //  for (int i = 0; i < motionCount; ++i)
    //    weights[i] = FP._0;
    //}

    public void CalculateWeightTable(int res)
    {
      resolution = res;
      int xLength = res;
      int yLength = Is2D() ? res : 1;
      int zLength = positions.Length;

      weightTable = new FP[xLength, yLength][];

      if (!Is2D())
      {//1D
        for (int x = 0; x < xLength; x++)
        {
          //FP xTarget = ((FP)x) / (((FP)xLength) - FP._1) - FP._1 * FP._2;
          FP xTarget = ((FP)x / ((FP)xLength - FP._1)) * FP._2 - FP._1;
          FP[] weightValues = Calculate_1D(xTarget, positions);
          weightTable[x, 0] = new FP[zLength];
          for (int z = 0; z < zLength; z++)
          {
            weightTable[x, 0][z] = weightValues[z];
          }
        }
      }
      else
      {//2d
        FP xDiv = xLength - 1;
        FP yDiv = yLength - 1;
        for (int x = 0; x < xLength; x++)
        {
          for (int y = 0; y < yLength; y++)
          {

            FP blendParameterX = ((FP)x / xDiv) * FP._2 - FP._1;
            FP blendParameterY = ((FP)y / yDiv) * FP._2 - FP._1;
            FPVector2 fix = new FPVector2(blendParameterX, blendParameterY);
            FP[] weightValues = calculate_2d(fix, positions);
            weightTable[x, y] = new FP[zLength];
            for (int z = 0; z < zLength; z++)
            {
              weightTable[x, y][z] = weightValues[z];
            }
          }
        }
      }
    }

    private FP[] Calculate_1D(FP targetPoint, FPVector2[] points)
    {
      int pointCount = points.Length;
      FP[] output = new FP[pointCount];
      FP closest = FP._1;
      int closestIndexA = 1;
      int closestIndexB = -1;
      for (int i = 0; i < pointCount; ++i)
      {
        output[i] = FP._0;
        //find negative
        FP point = points[i].X;

        FP comparePoint = FPMath.Abs(targetPoint - point);
        if (closest > comparePoint)
        {
          closest = comparePoint;
          closestIndexB = closestIndexA;
          closestIndexA = i;
        }
      }

      if (closestIndexA == -1)
      {
        closestIndexA = pointCount - 1;
        closestIndexB = pointCount - 2;
        closest = FP._0;
      }
      if (closestIndexB == -1 || (closestIndexA == 1 && closestIndexB == 1))
      {
        output[closestIndexA] = FP._1;
        return output;
      }

      FP totalWeight = FP._0;
      output[closestIndexA] = FP._1 - FPMath.Clamp(closest, FP._0, FP._1);
      totalWeight += output[closestIndexA];
      output[closestIndexB] = FPMath.Clamp(closest, FP._0, FP._1);
      totalWeight += output[closestIndexB];

      output[closestIndexA] = output[closestIndexA] / totalWeight;
      output[closestIndexB] = output[closestIndexB] / totalWeight;

      return output;
    }

    private FP[] calculate_2d(FPVector2 targetPoint, FPVector2[] points)
    {
      int pointCount = points.Length;
      FP[] output = new FP[pointCount];
      FP totalWeight = FP._0;

      for (int i = 0; i < pointCount; ++i)
      {
        FPVector2 firstPoint = points[i];
        FPVector2 comparePoint = targetPoint - firstPoint;

        FP weight = FP._1;

        for (int j = 0; j < pointCount; ++j)
        {
          if (j == i)
            continue;

          FPVector2 secondPoint = points[j] - firstPoint;
          FP sqrLen = FPVector2.Dot(secondPoint, secondPoint);
          if (sqrLen > FP.Epsilon)
            weight = FPMath.Min(weight, FPMath.Clamp((FP._1 - FPVector2.Dot(comparePoint, secondPoint) / sqrLen), FP._0, FP._1));
        }

        output[i] = weight;
        totalWeight += weight;
      }

      if (totalWeight > FP.Epsilon)
        for (int i = 0; i < pointCount; ++i)
          output[i] = output[i] / totalWeight;

      return output;
    }
  }

  //class that we will use for serialization
  [Serializable]
  public struct SerializableMotion
  {
    public bool isAnimation;
    public string name;
    public AnimatorData animatorData;

    public int blendParameterIndex;
    public int blendParameterIndexY;
    public FPVector2[] positions;

    public int resolution;
    public SerializableWeightDimentionX weightTable;

    public int childCount;
    public int indexOfFirstChild;
  }

  [Serializable]
  public struct SerializableWeightDimentionX
  {
    public SerializableWeightDimentionY[] data;
  }

  [Serializable]
  public struct SerializableWeightDimentionY
  {
    public SerializableWeightDimentionZ[] data;
  }

  [Serializable]
  public struct SerializableWeightDimentionZ
  {
    public FP[] data;
  }
}
