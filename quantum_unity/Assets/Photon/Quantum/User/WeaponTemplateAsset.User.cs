using UnityEngine;

public partial class WeaponTemplateAsset : InfoAssetAsset
{
    [Header("In-Game")]

    public GameObject Main;
    public GameObject Alt;
    public GameObject Preview;

    public Vector3 IconCameraPosition;
    public Vector3 IconCameraRotation;
}
