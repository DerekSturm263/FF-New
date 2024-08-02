using Extensions.Miscellaneous;
using UnityEngine;

public class WeaponEnhancerPopulator : PopulateAsset<WeaponEnhancerAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Enhancers";

    protected override void Decorate(GameObject buttonObj, WeaponEnhancerAsset item)
    {
        base.Decorate(buttonObj, item);

        bool infinite = InventoryController.Instance.HasInfiniteItem(item);
        int countNum = InventoryController.Instance.GetItemCount(item);

        TMPro.TMP_Text count = buttonObj.FindChildWithTag("Count", false)?.GetComponent<TMPro.TMP_Text>();

        if (count)
        {
            if (infinite)
            {
                count.gameObject.SetActive(false);
                return;
            }

            count.SetText(countNum.ToString());

            if (countNum == 0)
                count.color = Color.red;
        }
    }

    protected override bool GiveEvents(WeaponEnhancerAsset item) => InventoryController.Instance.HasItem(item);

    protected override bool IsEquipped(WeaponEnhancerAsset item) => false;
}
