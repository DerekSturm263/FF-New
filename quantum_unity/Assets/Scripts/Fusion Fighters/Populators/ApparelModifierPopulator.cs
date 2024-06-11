using Extensions.Miscellaneous;
using UnityEngine;

public class ApparelModifierPopulator : PopulateAsset<ApparelModifierAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Apparel/Modifiers";

    protected override bool DoSpawn(ApparelModifierAsset item) => InventoryController.Instance.HasItem(item);

    protected override void Decorate(GameObject buttonObj, ApparelModifierAsset item)
    {
        base.Decorate(buttonObj, item);

        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count")?.GetComponent<TMPro.TMP_Text>();
        if (count)
            count.SetText(countNum.ToString());

        if (countNum == 0)
            count.color = Color.red;
    }

    protected override bool GiveEvents(ApparelModifierAsset item) => InventoryController.Instance.GetItemCount(item) > 0;
}
