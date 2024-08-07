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
    [SerializeField] private Dictionary<AssetRefFFAvatar, Transform> _rootSlot;

    [SerializeField] private Dictionary<AssetRefFFAvatar, SkinnedMeshRenderer> _headSlot;
    [SerializeField] private Dictionary<AssetRefFFAvatar, ParentClothing> _parentClothingSlot;

    private Transform _altWeapon, _mainWeapon, _subWeapon, _headgear, _clothing, _legwear, _hair, _root;
    private ParentClothing _parentClothing;

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

    }

    private void SetAvatar(EventOnPlayerSetAvatar e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (e.New.Avatar == _female)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (e.New.Avatar == _male)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        _altWeapon = _altWeaponSlot[e.New.Avatar];
        _mainWeapon = _mainWeaponSlot[e.New.Avatar];
        _subWeapon = _subWeaponSlot[e.New.Avatar];
        _headgear = _headgearSlot[e.New.Avatar];
        _clothing = _clothingSlot[e.New.Avatar];
        _legwear = _legwearSlot[e.New.Avatar];
        _hair = _hairSlot[e.New.Avatar];
        _head = _headSlot[e.New.Avatar];
        _parentClothing = _parentClothingSlot[e.New.Avatar];
        _root = _rootSlot[e.New.Avatar];

        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(e.New.Color.Id);
        if (color)
        {
            _head.materials[0].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
        }

        if (_currentAltWeapon)
            _currentAltWeapon.transform.SetParent(_altWeapon, false);
        
        if (_currentMainWeapon)
            _currentMainWeapon.transform.SetParent(_mainWeapon, false);

        if (_currentSubWeapon)
            _currentSubWeapon.transform.SetParent(_subWeapon, false);

        if (_currentHeadgear)
        {
            _currentHeadgear.transform.SetParent(_headgear, false);
            _currentHeadgear.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }

        if (_currentClothing)
        {
            _currentClothing.transform.SetParent(_clothing, false);
            _currentClothing.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }

        if (_currentLegwear)
        {
            _currentLegwear.transform.SetParent(_legwear, false);
            _currentLegwear.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }

        if (_currentHair)
            _currentHair.transform.SetParent(_hair, false);

        //_currentHair.GetComponent<SkinnedMeshRenderer>().rootBone = _root;
    }

    private void SetClothing(EventOnPlayerSetClothing e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentClothing)
            Destroy(_currentClothing);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template && template.Instance)
        {
            _currentClothing = Instantiate(template.Instance, _clothing);
            _currentClothing.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }
    }

    private void SetHeadgear(EventOnPlayerSetHeadgear e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentHeadgear)
            Destroy(_currentHeadgear);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template && template.Instance)
        {
            _currentHeadgear = Instantiate(template.Instance, _headgear);
            _currentHeadgear.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }
    }

    private void SetLegwear(EventOnPlayerSetLegwear e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentLegwear)
            Destroy(_currentLegwear);

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(e.New.Template.Id);
        if (template && template.Instance)
        {
            _currentLegwear = Instantiate(template.Instance, _legwear);
            _currentLegwear.GetComponent<ParentClothing>().SetParent(_parentClothing);
        }
    }

    private void SetEyes(EventOnPlayerSetEyes e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        EyesAsset eyes = UnityDB.FindAsset<EyesAsset>(e.New.Eyes.Id);
        if (eyes && eyes.Texture)
        {
            _head.materials[2].SetTexture("_Base_Map", eyes.Texture);
        }

        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(e.New.Color.Id);
        if (color)
        {
            _head.materials[2].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
        }
    }

    private void SetHair(EventOnPlayerSetHair e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentHair)
            Destroy(_currentHair);

        HairAsset template = UnityDB.FindAsset<HairAsset>(e.New.Hair.Id);
        if (template && template.Hair)
        {
            _currentHair = Instantiate(template.Hair, _hair);
        }
        
        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(e.New.Color.Id);
        if (color)
        {
            _currentHair.GetComponentInChildren<MeshRenderer>().materials[0].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
        }
    }

    private void SetMainWeapon(EventOnPlayerSetMainWeapon e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

    }

    private void SetSub(EventOnPlayerSetSub e)
    {
        if (e.Player != _entityView.EntityRef)
            return;

        if (_currentSubWeapon)
            Destroy(_currentSubWeapon);

        SubTemplateAsset template = UnityDB.FindAsset<SubTemplateAsset>(e.New.Template.Id);
        if (template && template.Instance)
        {
            _currentSubWeapon = Instantiate(template.Instance, _subWeapon);
        }
    }
}
