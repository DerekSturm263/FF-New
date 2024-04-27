using Photon.Deterministic;

namespace Quantum.Custom.Animator
{

  public struct AnimatorRuntimeBlendData
  {
    public int layerId;
    public int stateId;
    public int animationIndex;
    public FP currentTime;
    public FP normalTime;
    public FP lastTime;
    public FP weight;
    public FP length;
    public FP calculatedLength;

    public AnimatorRuntimeBlendData(AnimatorLayer layer, AnimatorState state, int index, FP lastTime, FP currentTime, FP normalisedTime, FP weight, FP length, FP calculatedLength)
    {
      layerId = layer.id;
      stateId = state.id;
      animationIndex = index;
      this.currentTime = currentTime;
      normalTime = normalisedTime;
      this.lastTime = lastTime;
      this.weight = weight;
      this.length = length;
      this.calculatedLength = calculatedLength;
    }
  }
}
