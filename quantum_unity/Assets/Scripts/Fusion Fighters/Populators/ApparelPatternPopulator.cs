using Extensions.Miscellaneous;
using UnityEngine;

public class ApparelPatternPopulator : PopulateAsset<ApparelPatternAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Apparel/Patterns";

    protected override void Decorate(GameObject buttonObj, ApparelPatternAsset item)
    {
        base.Decorate(buttonObj, item);

        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count", false)?.GetComponent<TMPro.TMP_Text>();
        if (count)
            count.SetText(countNum.ToString());

        if (countNum == 0)
            count.color = Color.red;
    }

    protected override bool GiveEvents(ApparelPatternAsset item) => InventoryController.Instance.GetItemCount(item) > 0;

    protected override bool IsEquipped(ApparelPatternAsset item) => false;
}
