using Extensions.Components.UI;
using Quantum;

public class DisplayStageInfo : DisplayText<Stage>
{
    protected override string GetInfo(Stage item) => item.SerializableData.Name;
    protected override Stage GetValue() => default;
}
