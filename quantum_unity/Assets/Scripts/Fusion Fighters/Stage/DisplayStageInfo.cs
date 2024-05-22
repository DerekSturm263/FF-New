using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

using Type = Quantum.Stage;

public class DisplayStageInfo : Display<Type, Extensions.Types.Tuple<UnityEvent<string>, UnityEvent<string>>>
{
    [SerializeField] protected string _format = "{0}";

    protected (string, string) GetInfo(Type item) => (item.SerializableData.Name, item.SerializableData.Description);
    protected override Type GetValue() => default;
    public override void UpdateDisplay(Type item)
    {
        var info = GetInfo(item);

        _component.Item1.Invoke(string.Format(_format, info.Item1));
        _component.Item2.Invoke(string.Format(_format, info.Item2));
    }
}
