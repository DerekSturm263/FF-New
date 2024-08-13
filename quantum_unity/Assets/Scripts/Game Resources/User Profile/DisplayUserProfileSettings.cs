using Extensions.Components.UI;
using UnityEngine;
using UnityEngine.Events;

using Type = SerializableWrapper<UserProfile>;

public class DisplayUserProfileSettings : Display<Type, Extensions.Types.Tuple<UnityEvent<string>, UnityEvent<float>>>
{
    protected (string[], Sprite) GetInfo(Type item) => (new string[] { item.Name, item.Description }, item.Preview);
    public override void UpdateDisplay(Type item)
    {
        var info = GetInfo(item);

        _component.Item1.Invoke(item.Name);
        _component.Item2.Invoke(item.value.HapticStrength);
    }

    protected override Type GetValue() => (UserProfileController.Instance as UserProfileController).CurrentlySelected;
}
