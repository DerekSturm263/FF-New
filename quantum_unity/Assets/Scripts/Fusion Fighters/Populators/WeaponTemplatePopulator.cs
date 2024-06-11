using Extensions.Miscellaneous;
using UnityEngine;

public class WeaponTemplatePopulator : PopulateAsset<WeaponTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Templates";

    protected override bool DoSpawn(WeaponTemplateAsset item) => InventoryController.Instance.HasItem(item);

    protected override void Decorate(GameObject buttonObj, WeaponTemplateAsset item)
    {
        base.Decorate(buttonObj, item);

        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count")?.GetComponent<TMPro.TMP_Text>();
        if (count)
            count.SetText(countNum.ToString());

        if (countNum == 0)
            count.color = Color.red;
    }

    protected override bool GiveEvents(WeaponTemplateAsset item) => InventoryController.Instance.GetItemCount(item) > 0;
}
