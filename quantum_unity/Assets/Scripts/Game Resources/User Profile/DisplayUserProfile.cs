using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

using Type = SerializableWrapper<UserProfile>;

public class DisplayUserProfile : Display<Type, Extensions.Types.Tuple<UnityEvent<string>[], UnityEvent<Sprite>>>
{
    [SerializeField] protected string _format = "{0}";

    protected (string[], Sprite) GetInfo(Type item) => (new string[] { item.Name, item.Description }, item.Preview);
    public override void UpdateDisplay(Type item)
    {
        var info = GetInfo(item);

        _component.Item1[0].Invoke(string.Format(_format, info.Item1[0]));
    }

    protected override Type GetValue() => (UserProfileController.Instance as UserProfileController).CurrentlySelected;
}
