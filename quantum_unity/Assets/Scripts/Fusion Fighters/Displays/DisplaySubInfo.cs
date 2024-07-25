using Extensions.Components.UI;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Sub>;

public class DisplaySubInfo : DisplayBase
{
    [SerializeField] private DisplaySubTemplateInfo _component1;
    [SerializeField] private DisplaySubEnhancerInfo _component2;

    [SerializeField] private AssetRefSubEnhancer _none;

    public override void UpdateDisplayOnEnable() => UpdateDisplay();

    protected Type GetValue() => default;

    public void UpdateDisplay(Type item)
    {
        _component1.UpdateDisplay(UnityDB.FindAsset<SubTemplateAsset>(item.Value.Template.Id));

        if (item.Value.Enhancer.Id.IsValid)
            _component2.UpdateDisplay(UnityDB.FindAsset<SubEnhancerAsset>(item.Value.Enhancer.Id));
        else
            _component2.UpdateDisplay(UnityDB.FindAsset<SubEnhancerAsset>(_none.Id));
    }

    public void UpdateDisplay() => UpdateDisplay(GetValue());
}
