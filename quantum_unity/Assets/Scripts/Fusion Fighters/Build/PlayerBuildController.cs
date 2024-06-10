using Quantum;
using UnityEngine;

public class PlayerBuildController : MonoBehaviour
{
    [SerializeField] private AssetRefFFAvatar _female;
    [SerializeField] private AssetRefFFAvatar _male;

    [SerializeField] private Transform _altWeaponSlot;
    [SerializeField] private Transform _mainWeaponSlot;
    [SerializeField] private Transform _subWeaponSlot;
    [SerializeField] private Transform _headgearSlot;
    [SerializeField] private Transform _clothingSlot;
    [SerializeField] private Transform _legwearSlot;

    [SerializeField] private SkinnedMeshRenderer _head;

    private GameObject _currentAltWeapon;
    private GameObject _currentSubWeapon;
    private GameObject _currentMainWeapon;

    private EntityView _entityView;

    private void Awake()
    {
        _entityView = GetComponent<EntityView>();

        QuantumEvent.Subscribe<EventOnPlayerSetAltWeapon>(listener: this, handler: SetAltWeapon);
        QuantumEvent.Subscribe<EventOnPlayerSetAvatar>(listener: this, handler: SetAvatar);
        //QuantumEvent.Subscribe<EventOnPlayerSetBadge>(listener: this, handler: default);
        QuantumEvent.Subscribe<EventOnPlayerSetClothing>(listener: this, handler: SetClothing);
        QuantumEvent.Subscribe<EventOnPlayerSetHeadgear>(listener: this, handler: SetHeadgear);
        QuantumEvent.Subscribe<EventOnPlayerSetLegwear>(listener: this, handler: SetLegwear);
        //QuantumEvent.Subscribe<EventOnPlayerSetEmoteDown>(listener: this, handler: default);
        //QuantumEvent.Subscribe<EventOnPlayerSetEmoteUp>(listener: this, handler: default);
        //QuantumEvent.Subscribe<EventOnPlayerSetEmoteLeft>(listener: this, handler: default);
        //QuantumEvent.Subscribe<EventOnPlayerSetEmoteRight>(listener: this, handler: default);
        QuantumEvent.Subscribe<EventOnPlayerSetEyes>(listener: this, handler: SetEyes);
        QuantumEvent.Subscribe<EventOnPlayerSetHair>(listener: this, handler: SetHair);
        QuantumEvent.Subscribe<EventOnPlayerSetMainWeapon>(listener: this, handler: SetMainWeapon);
        QuantumEvent.Subscribe<EventOnPlayerSetSub>(listener: this, handler: SetSub);
        //QuantumEvent.Subscribe<EventOnPlayerSetUltimate>(listener: this, handler: default);
        //QuantumEvent.Subscribe<EventOnPlayerSetVoice>(listener: this, handler: default);
    }

    private void SetAltWeapon(EventOnPlayerSetAltWeapon e)
    {
        if (_currentAltWeapon)
            Destroy(_currentAltWeapon);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentAltWeapon = Instantiate(template.Weapon, _altWeaponSlot);
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
    }

    private void SetClothing(EventOnPlayerSetClothing e)
    {

    }

    private void SetHeadgear(EventOnPlayerSetHeadgear e)
    {

    }

    private void SetLegwear(EventOnPlayerSetLegwear e)
    {

    }

    private void SetEyes(EventOnPlayerSetEyes e)
    {
        _head.materials[2].SetTexture("_Base_Map", UnityDB.FindAsset<EyesAsset>(e.New.Id).Texture);
    }

    private void SetHair(EventOnPlayerSetHair e)
    {

    }

    private void SetMainWeapon(EventOnPlayerSetMainWeapon e)
    {
        if (_currentMainWeapon)
            Destroy(_currentMainWeapon);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentMainWeapon = Instantiate(template.Weapon, _mainWeaponSlot);
        }
    }

    private void SetSub(EventOnPlayerSetSub e)
    {
        if (_currentSubWeapon)
            Destroy(_currentSubWeapon);

        SubTemplateAsset template = UnityDB.FindAsset<SubTemplateAsset>(e.New.Template.Id);
        if (template)
        {
            _currentSubWeapon = Instantiate(template.Weapon, _subWeaponSlot);
        }
    }
}
