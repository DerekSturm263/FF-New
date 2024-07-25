using Extensions.Components.UI;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Apparel>;

public class DisplayApparelInfo : DisplayBase
{
    [SerializeField] private DisplayApparelTemplateInfo _component1;
    [SerializeField] private DisplayApparelPatternInfo _component2;
    [SerializeField] private DisplayApparelModifierInfo _component3;
    [SerializeField] private DisplayApparelModifierInfo _component4;
    [SerializeField] private DisplayApparelModifierInfo _component5;

    [SerializeField] private AssetRefApparelModifier _none;

    public override void UpdateDisplayOnEnable() => UpdateDisplay();

    protected Type GetValue() => default;

    public void UpdateDisplay(Type item)
    {
        _component1.UpdateDisplay(UnityDB.FindAsset<ApparelTemplateAsset>(item.value.Template.Id));
        _component2.UpdateDisplay(UnityDB.FindAsset<ApparelPatternAsset>(item.value.Pattern.Id));

        if (item.value.Modifiers.Modifier1.Id.IsValid)
            _component3.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(item.value.Modifiers.Modifier1.Id));
        else
            _component3.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(_none.Id));

        if (item.value.Modifiers.Modifier2.Id.IsValid)
            _component4.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(item.value.Modifiers.Modifier2.Id));
        else
            _component4.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(_none.Id));

        if (item.value.Modifiers.Modifier3.Id.IsValid)
            _component5.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(item.value.Modifiers.Modifier3.Id));
        else
            _component5.UpdateDisplay(UnityDB.FindAsset<ApparelModifierAsset>(_none.Id));
    }

    public void UpdateDisplay() => UpdateDisplay(GetValue());
}
