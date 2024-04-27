using System;
using Photon.Deterministic;

namespace Quantum.Custom.Animator
{

  /// <summary>
  /// Contains everythig related to the conditions used on the transitions between states
  /// You'll find here the checks depending on the type of check and on the data type
  /// </summary>
  [Serializable]
  public unsafe class AnimatorCondition
  {

    public string variableName = "";
    public Modes mode;
    public FP thresholdFp;
    public Int32 thresholdInt;

    /// <summary>
    /// Mapped 1 to 1 with AnimatorConditionMode
    /// </summary>
    public enum Modes
    {
      If = 1,
      IfNot = 2,
      Greater = 3,
      Less = 4,
      Equals = 6,
      NotEqual = 7,
    }

    public AnimatorCondition()
    {
      thresholdFp = FP._0;
      thresholdInt = 0;
    }

    public bool Check(Frame f, CustomAnimator* a, CustomAnimatorGraph graph)
    {
      int variable_index = graph.VariableIndex(variableName);
      if (variable_index == -1)
      {
        Log.Error(string.Format("Variable not found in graph: {0}", variableName));
      }

      AnimatorVariable variable = graph.variables[variable_index];

      switch (variable.type)
      {
        case AnimatorVariable.VariableType.FP:
          switch (mode)
          {
            case Modes.Equals:
              return *CustomAnimator.Variable(f, a, variable_index)->FPValue == thresholdFp;
            case Modes.Greater:
              return *CustomAnimator.Variable(f, a, variable_index)->FPValue > thresholdFp;
            case Modes.Less:
              return *CustomAnimator.Variable(f, a, variable_index)->FPValue < thresholdFp;
            case Modes.NotEqual:
              return *CustomAnimator.Variable(f, a, variable_index)->FPValue != thresholdFp;
          }

          break;

        case AnimatorVariable.VariableType.Int:
          switch (mode)
          {
            case Modes.Equals:
              return *CustomAnimator.Variable(f, a, variable_index)->IntegerValue == thresholdInt;
            case Modes.Greater:
              return *CustomAnimator.Variable(f, a, variable_index)->IntegerValue > thresholdInt;
            case Modes.Less:
              return *CustomAnimator.Variable(f, a, variable_index)->IntegerValue < thresholdInt;
            case Modes.NotEqual:
              return *CustomAnimator.Variable(f, a, variable_index)->IntegerValue != thresholdInt;
          }

          break;

        case AnimatorVariable.VariableType.Bool:
          switch (mode)
          {
            case Modes.If:
              return *CustomAnimator.Variable(f, a, variable_index)->BooleanValue;
            case Modes.IfNot:
              return !*CustomAnimator.Variable(f, a, variable_index)->BooleanValue;
          }

          break;

        case AnimatorVariable.VariableType.Trigger:
          switch (mode)
          {
            case Modes.If:
              return *CustomAnimator.Variable(f, a, variable_index)->BooleanValue;
          }

          break;
      }

      return false;
    }
  }
}
