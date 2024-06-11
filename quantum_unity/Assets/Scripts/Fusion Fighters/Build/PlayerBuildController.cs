using Extensions.Types;
using Quantum;
using UnityEngine;

public class PlayerBuildController : MonoBehaviour
{
    [SerializeField] private AssetRefFFAvatar _female;
    [SerializeField] private AssetRefFFAvatar _male;

    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _altWeaponSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _mainWeaponSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _subWeaponSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _headgearSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _clothingSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _legwearSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _hairSlot;

    [SerializeField] private Dictionary<AssetRefFFAvatar, SkinnedMeshRenderer> _headSlot;

    private Transform _altWeapon, _mainWeapon, _subWeapon, _headgear, _clothing, _legwear, _hair;

    private GameObject _currentAltWeapon;
    private GameObject _currentSubWeapon;
    private GameObject _currentMainWeapon;
    private GameObject _currentHeadgear;
    private GameObject _currentClothing;
    private GameObject _currentLegwear;
    private GameObject _currentHair;
    private SkinnedMeshRenderer _head;

    private EntityView _entityView;

    private void Awake()
    {
        _entityView = GetComponent<EntityView>();

        QuantumEvent.Subscribe<EventOnPlayerSetAltWeapon>(listener: this, handler: SetAltWeapon);
        QuantumEvent.Subscribe<EventOnPlayerSetAvatar>(listener: this, handler: SetAvatar);
        QuantumEvent.Subscribe<EventOnPlayerSetClothing>(listener: this, handler: SetClothing);
        QuantumEvent.Subscribe<EventOnPlayerSetHeadgear>(listener: this, handler: SetHeadgear);
        QuantumEvent.Subscribe<EventOnPlayerSetLegwear>(listener: this, handler: SetLegwear);
        QuantumEvent.Subscribe<EventOnPlayerSetEyes>(listener: this, handler: SetEyes);
        QuantumEvent.Subscribe<EventOnPlayerSetHair>(listener: this, handler: SetHair);
        QuantumEvent.Subscribe<EventOnPlayerSetMainWeapon>(listener: this, handler: SetMainWeapon);
        QuantumEvent.Subscribe<EventOnPlayerSetSub>(listener: this, handler: SetSub);
    }

    private void SetAltWeapon(EventOnPlayerSetAltWeapon e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentAltWeapon)
            Destroy(_currentAltWeapon);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentAltWeapon = Instantiate(template.Weapon, _altWeapon);
        }
    }

    private void SetAvatar(EventOnPlayerSetAvatar e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (e.New == _female)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (e.New == _male)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        _altWeapon = _altWeaponSlot[e.New];
        _mainWeapon = _mainWeaponSlot[e.New];
        _subWeapon = _subWeaponSlot[e.New];
        _headgear = _headgearSlot[e.New];
        _clothing = _clothingSlot[e.New];
        _legwear = _legwearSlot[e.New];
        _hair = _hairSlot[e.New];
        _head = _headSlot[e.New];
    }

    private void SetClothing(EventOnPlayerSetClothing e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentClothing)
            Destroy(_currentClothing);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentClothing = Instantiate(template.Apparel, _clothing);
        }
    }

    private void SetHeadgear(EventOnPlayerSetHeadgear e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentHeadgear)
            Destroy(_currentHeadgear);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentHeadgear = Instantiate(template.Apparel, _headgear);
        }
    }

    private void SetLegwear(EventOnPlayerSetLegwear e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentLegwear)
            Destroy(_currentLegwear);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentLegwear = Instantiate(template.Apparel, _legwear);
        }
    }

    private void SetEyes(EventOnPlayerSetEyes e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        _head.materials[2].SetTexture("_Base_Map", UnityDB.FindAsset<EyesAsset>(e.New.Id).Texture);
    }

    private void SetHair(EventOnPlayerSetHair e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentHair)
            Destroy(_currentHair);

        HairAsset template = UnityDB.FindAsset<HairAsset>(e.New.Id);
        if (template)
        {
            _currentHair = Instantiate(template.Hair, _hair);
        }
    }

    private void SetMainWeapon(EventOnPlayerSetMainWeapon e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentMainWeapon)
            Destroy(_currentMainWeapon);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentMainWeapon = Instantiate(template.Weapon, _mainWeapon);
        }
    }

    private void SetSub(EventOnPlayerSetSub e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentSubWeapon)
            Destroy(_currentSubWeapon);

        SubTemplateAsset template = UnityDB.FindAsset<SubTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentSubWeapon = Instantiate(template.Weapon, _subWeapon);
        }
    }
}
