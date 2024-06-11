using Extensions.Miscellaneous;
using UnityEngine;

public class SubEnhancerPopulator : PopulateAsset<SubEnhancerAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Enhancers";

    protected override bool DoSpawn(SubEnhancerAsset item) => InventoryController.Instance.HasItem(item);

    protected override void Decorate(GameObject buttonObj, SubEnhancerAsset item)
    {
        base.Decorate(buttonObj, item);

        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count")?.GetComponent<TMPro.TMP_Text>();
        if (count)
            count.SetText(countNum.ToString());

        if (countNum == 0)
            count.color = Color.red;
    }

    protected override bool GiveEvents(SubEnhancerAsset item) => InventoryController.Instance.GetItemCount(item) > 0;
}
