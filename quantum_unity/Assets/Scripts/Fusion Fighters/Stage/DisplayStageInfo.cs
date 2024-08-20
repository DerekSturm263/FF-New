using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

using Type = SerializableWrapper<Quantum.Stage>;

public class DisplayStageInfo : Display<Type, Extensions.Types.Tuple<UnityEvent<string>[], UnityEvent<Sprite>>>
{
    [SerializeField] protected string _format = "{0}";
    [SerializeField] private GameObject _sprite;

    [SerializeField] private bool _defaultIsNone = false;

    protected (string[], Sprite) GetInfo(Type item) => (new string[] { item.Name, item.Description }, item.Preview);
    public override void UpdateDisplay(Type item)
    {
        var info = GetInfo(item);

        if (_sprite)
            _sprite.SetActive(info.Item2);

        _component.Item1[0].Invoke(string.Format(_format, info.Item1[0]));
        _component.Item1[1].Invoke(string.Format(_format, info.Item1[1]));

        _component.Item2.Invoke(info.Item2);
    }

    protected override Type GetValue() => _defaultIsNone ? default : StageController.Instance.CurrentStage;
}
