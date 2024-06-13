using Extensions.Miscellaneous;
using UnityEngine;

public class WeaponEnhancerPopulator : PopulateAsset<WeaponEnhancerAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Enhancers";

    protected override bool DoSpawn(WeaponEnhancerAsset item) => InventoryController.Instance.HasItem(item);

    protected override void Decorate(GameObject buttonObj, WeaponEnhancerAsset item)
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
    }

    protected override bool GiveEvents(WeaponEnhancerAsset item) => InventoryController.Instance.GetItemCount(item) == -1 || InventoryController.Instance.GetItemCount(item) > 0;
}
