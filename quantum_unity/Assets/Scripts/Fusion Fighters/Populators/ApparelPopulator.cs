using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApparelPopulator : PopulateSerializable<Quantum.Apparel, ApparelAssetAsset>
{
    [SerializeField] private ApparelAssetAsset _none;
    [SerializeField] private Quantum.ApparelTemplate.ApparelType _type;

    protected override string BuiltInFilePath() => "DB/Assets/Build/Equipment/Apparel/Apparel";
    protected override string CustomFilePath() => ApparelController.GetPath();

    protected override SerializableWrapper<Quantum.Apparel> GetFromBuiltInAsset(ApparelAssetAsset asset)
    {
        var item = asset.Apparel;
        item.SetIconForBuiltIn(asset.Icon);

        return item;
    }

    protected override IEnumerable<SerializableWrapper<Quantum.Apparel>> LoadAll()
    {
        List<SerializableWrapper<Quantum.Apparel>> results = new();

        if (_loadType.HasFlag(CreationType.BuiltIn))
            results.AddRange(Resources.LoadAll<ApparelAssetAsset>(BuiltInFilePath()).Where(item => item.IncludeInLists).Select(GetFromBuiltInAsset));

        if (_loadType.HasFlag(CreationType.Custom))
            results.AddRange(FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<Quantum.Apparel>>(CustomFilePath()).Where(item => UnityDB.FindAsset<ApparelTemplateAsset>(item.value.Template.Id).Settings_ApparelTemplate.Type.HasFlag(_type)));

        return results;
    }

    protected override bool IsEquipped(SerializableWrapper<Quantum.Apparel> item)
    {
        return _type switch
        {
            Quantum.ApparelTemplate.ApparelType.Headgear => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Headgear.Equals(item.value),
            Quantum.ApparelTemplate.ApparelType.Clothing => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Clothing.Equals(item.value),
            Quantum.ApparelTemplate.ApparelType.Legwear => BuildController.Instance.CurrentBuild.value.Equipment.Outfit.Legwear.Equals(item.value),
            _ => false
        };
    }
    protected override bool IsNone(SerializableWrapper<Quantum.Apparel> item) => item.value.Equals(_none.Apparel.value);
}
