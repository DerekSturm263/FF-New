using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

using Type = SerializableWrapper<Quantum.Stage>;

public class DisplayStageInfo : Display<Type, Extensions.Types.Tuple<UnityEvent<string>[], UnityEvent<Sprite>>>
{
    [SerializeField] protected string _format = "{0}";

    protected (string[], Sprite) GetInfo(Type item) => (new string[] { item.Value.SerializableData.Name, item.Value.SerializableData.Description }, item.Preview);
    protected override Type GetValue() => default;
    public override void UpdateDisplay(Type item)
    {
        var info = GetInfo(item);

        _component.Item1[0].Invoke(string.Format(_format, info.Item1[0]));
        _component.Item1[1].Invoke(string.Format(_format, info.Item1[1]));

        _component.Item2.Invoke(info.Item2);
    }
}
