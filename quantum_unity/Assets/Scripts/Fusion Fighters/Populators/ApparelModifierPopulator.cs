using Extensions.Miscellaneous;
using UnityEngine;

public class ApparelModifierPopulator : PopulateAsset<ApparelModifierAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Apparel/Modifiers";

    protected override bool DoSpawn(ApparelModifierAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override void Decorate(GameObject buttonObj, ApparelModifierAsset item)
    {
        base.Decorate(buttonObj, item);

        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count")?.GetComponent<TMPro.TMP_Text>();
        if (count)
            count.SetText(countNum.ToString());

        if (countNum == 0)
            count.color = Color.red;
        else if (countNum == -1)
            count.gameObject.SetActive(false);

        TMPro.TMP_Text current = buttonObj.FindChildWithTag("Current")?.GetComponent<TMPro.TMP_Text>();
        current.color = new(0.7f, 0.7f, 0.7f);
    }

    protected override bool GiveEvents(ApparelModifierAsset item) => InventoryController.Instance.HasItem(item);
}
