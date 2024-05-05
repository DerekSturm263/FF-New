using System;
using Photon.Deterministic;

namespace Quantum.Custom.Animator
{
  [Serializable]
  public class AnimatorVariable {
    public enum VariableType {
      Int,
      FP,
      Bool,
      Trigger
    }

    public string name = "variable";

    public int index = 0;
    
    public VariableType type;

    public FP defaultFp;
    public Int32 defaultInt;
    public Boolean defaultBool;

    public object GetObject(int objectIndex) {
      if (objectIndex == index) {
        return this;
      }
      return null;
    }

    public override string ToString() {
      return name;
    }
  }
}
