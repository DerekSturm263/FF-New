using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventReceiverParent : MonoBehaviour
{
    private readonly List<Renderer> _meshRenderers = new();
    public List<Renderer> MeshRenderers => _meshRenderers;

    public void AddRenderer(Renderer renderer) => _meshRenderers.Add(renderer);
    public void RemoveRenderer(Renderer renderer) => _meshRenderers.Remove(renderer);

    private (GameObject, GameObject) _primaryWeaponInstance;
    public (GameObject, GameObject) PrimaryWeaponInstance => _primaryWeaponInstance;
    public void SetPrimaryWeaponInstance((GameObject, GameObject) weapons) => _primaryWeaponInstance = weapons;

    private (GameObject, GameObject) _secondaryWeaponInstance;
    public (GameObject, GameObject) SecondaryWeaponInstance => _secondaryWeaponInstance;
    public void SetSecondaryWeaponInstance((GameObject, GameObject) weapons) => _secondaryWeaponInstance = weapons;

    private GameObject _subInstance;
    public GameObject SubInstance => _subInstance;
    public void SetSubInstance(GameObject subInstance) => _subInstance = subInstance;

    private ParentClothing _headgearInstance;
    public ParentClothing HeadgearInstance => _headgearInstance;
    public void SetHeadgearInstance(ParentClothing headgearInstance) => _headgearInstance = headgearInstance;

    private ParentClothing _clothingInstance;
    public ParentClothing ClothingInstance => _clothingInstance;
    public void SetClothingInstance(ParentClothing clothingInstance) => _clothingInstance = clothingInstance;

    private ParentClothing _legwearInstance;
    public ParentClothing LegwearInstance => _legwearInstance;
    public void SetLegwearInstance(ParentClothing legwearInstance) => _legwearInstance = legwearInstance;

    private GameObject _hairInstance;
    public GameObject HairInstance => _hairInstance;
    public void SetHairInstance(GameObject hairInstance) => _hairInstance = hairInstance;

    private HurtboxSettings _hurtboxSettings;
    public HurtboxSettings HurtboxSettings => _hurtboxSettings;
    public void SetHurtboxSettings(HurtboxSettings hurtboxSettings) => _hurtboxSettings = hurtboxSettings;

    private AssetRefFFAvatar _avatar;
    public AssetRefFFAvatar Avatar => _avatar;
    public void SetAvatar(AssetRefFFAvatar avatar) => _avatar = avatar;

    private EyesColorBinding _eyes;
    public EyesColorBinding Eyes => _eyes;
    public void SetEyes(EyesColorBinding eyes) => _eyes = eyes;

    private int _direction;
    public int Direction => _direction;
    public void SetDirection(int direction) => _direction = direction;

    [SerializeField] private float _flashSpeed;
    public float FlashSpeed => _flashSpeed;

    [SerializeField] private FlashSettings _default, _invincible, _intangible, _superArmor;
    public FlashSettings Default => _default;
    public FlashSettings Invincible => _invincible;
    public FlashSettings Intangible => _intangible;
    public FlashSettings SuperArmor => _superArmor;
}
