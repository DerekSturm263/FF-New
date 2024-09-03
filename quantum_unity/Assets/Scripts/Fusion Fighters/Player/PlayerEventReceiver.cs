using Quantum;
using System.Collections;
using UnityEngine;

public class PlayerEventReceiver : MonoBehaviour
{
    private PlayerEventReceiverParent _parent;

    [SerializeField] private AssetRefFFAvatar _female, _male;

    [SerializeField] private Transform _primaryWeapon;
    [SerializeField] private Transform _secondaryWeapon;
    [SerializeField] private Transform _sub;
    [SerializeField] private Transform _hair;

    [SerializeField] private SkinnedMeshRenderer _head;

    private ParentClothing _clothingParent;

    [SerializeField] private Expression _defaultExpression;

    private Expression _expression;
    private float _expressionCurrentTime;
    private float _expressionMaxLength;

    private void Awake()
    {
        _parent = GetComponentInParent<PlayerEventReceiverParent>();
        _clothingParent = GetComponent<ParentClothing>();
    }

    private void Update()
    {
        UpdateHurtboxVisuals(Mathf.PingPong(Time.time * _parent.FlashSpeed, 1));
        UpdateExpression();
    }

    public void SetAvatar(AvatarColorBinding avatar, PlayerEventReceiver newAvatarObj, EntityRef entity)
    {
        foreach (Renderer skinnedMeshRenderer in GetComponentsInChildren<Renderer>())
            _parent.AddRenderer(skinnedMeshRenderer);

        if (newAvatarObj != this)
        {
            if (_parent.PrimaryWeaponInstance)
            {
                _parent.PrimaryWeaponInstance.transform.SetParent(newAvatarObj._primaryWeapon, false);
            }

            if (_parent.SecondaryWeaponInstance)
            {
                _parent.SecondaryWeaponInstance.transform.SetParent(newAvatarObj._secondaryWeapon, false);
            }

            if (_parent.SubInstance)
            {
                _parent.SubInstance.transform.SetParent(newAvatarObj._sub, false);
            }

            if (_parent.HeadgearInstance)
            {
                _parent.HeadgearInstance.transform.SetParent(newAvatarObj.transform, false);
                _parent.HeadgearInstance.SetParent(newAvatarObj._clothingParent);
            }

            if (_parent.ClothingInstance)
            {
                _parent.ClothingInstance.transform.SetParent(newAvatarObj.transform, false);
                _parent.ClothingInstance.SetParent(newAvatarObj._clothingParent);
            }

            if (_parent.LegwearInstance)
            {
                _parent.LegwearInstance.transform.SetParent(newAvatarObj.transform, false);
                _parent.LegwearInstance.SetParent(newAvatarObj._clothingParent);
            }

            if (_parent.HairInstance)
            {
                _parent.HairInstance.transform.SetParent(newAvatarObj._hair, false);
            }

            newAvatarObj.SetEyes(_parent.Eyes, entity);
        }

        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(avatar.Color.Id);
        if (color)
        {
            newAvatarObj._head.materials[0].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
            newAvatarObj._head.materials[0].SetFloat("_Smoothness", color.Settings_ColorPreset.Smoothness.AsFloat * 0.1f);
            newAvatarObj._head.materials[0].SetFloat("_Metallic", color.Settings_ColorPreset.Metallic.AsFloat);
            newAvatarObj._head.materials[0].SetFloat("_Fresnel_Strength", color.Settings_ColorPreset.FresnelStrength.AsFloat * 0.15f);
        }

        newAvatarObj.transform.GetChild(0).localScale = new Vector3(1, _parent.Direction, 1) * 100;

        _parent.SetAvatar(avatar.Avatar);
    }

    public void SetMainWeapon(Weapon mainWeapon, EntityRef entity)
    {
        if (_parent.PrimaryWeaponInstance)
            Destroy(_parent.PrimaryWeaponInstance);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(mainWeapon.Template.Id);
        if (template && template.Main)
        {
            _parent.SetPrimaryWeaponInstance(Instantiate(template.Main, _primaryWeapon));
        }

        WeaponMaterialAsset material = UnityDB.FindAsset<WeaponMaterialAsset>(mainWeapon.Material.Id);
        if (material)
        {
            _parent.PrimaryWeaponInstance.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_Base_Color", material.Color);
        }
    }

    public void SetAltWeapon(Weapon altWeapon, EntityRef entity)
    {
        if (_parent.SecondaryWeaponInstance)
            Destroy(_parent.SecondaryWeaponInstance);

        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(altWeapon.Template.Id);
        if (template && template.Alt)
        {
            _parent.SetSecondaryWeaponInstance(Instantiate(template.Alt, _secondaryWeapon));
        }
    }

    public void SetSub(Sub sub, EntityRef entity)
    {
        if (_parent.SubInstance)
            Destroy(_parent.SubInstance);

        SubTemplateAsset template = UnityDB.FindAsset<SubTemplateAsset>(sub.Template.Id);
        if (template && template.Instance)
        {
            _parent.SetSubInstance(Instantiate(template.Instance, _sub));
        }
    }

    public void SetHeadgear(Apparel headgear, EntityRef entity)
    {
        if (_parent.HeadgearInstance)
        {
            foreach (Renderer skinnedMeshRenderer in _parent.HeadgearInstance.GetComponentsInChildren<Renderer>())
                _parent.RemoveRenderer(skinnedMeshRenderer);

            Destroy(_parent.HeadgearInstance.gameObject);
        }

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(headgear.Template.Id);
        if (template && template.Instance)
        {
            _parent.SetHeadgearInstance(Instantiate(template.Instance, transform).GetComponent<ParentClothing>());
            _parent.HeadgearInstance.SetParent(_clothingParent);

            foreach (Renderer skinnedMeshRenderer in _parent.HeadgearInstance.GetComponentsInChildren<Renderer>())
                _parent.AddRenderer(skinnedMeshRenderer);

            _parent.HeadgearInstance.transform.GetChild(0).localScale = new Vector3(1, _parent.Direction, 1) * 100;
        }
    }

    public void SetClothing(Apparel clothing, EntityRef entity)
    {
        if (_parent.ClothingInstance)
        {
            foreach (Renderer skinnedMeshRenderer in _parent.ClothingInstance.GetComponentsInChildren<Renderer>())
                _parent.RemoveRenderer(skinnedMeshRenderer);

            Destroy(_parent.ClothingInstance.gameObject);
        }

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(clothing.Template.Id);
        if (template && template.Instance)
        {
            _parent.SetClothingInstance(Instantiate(template.Instance, transform).GetComponent<ParentClothing>());
            _parent.ClothingInstance.SetParent(_clothingParent);

            foreach (Renderer skinnedMeshRenderer in _parent.ClothingInstance.GetComponentsInChildren<Renderer>())
                _parent.AddRenderer(skinnedMeshRenderer);

            _parent.ClothingInstance.transform.GetChild(0).localScale = new Vector3(1, _parent.Direction, 1) * 100;
        }
    }

    public void SetLegwear(Apparel legwear, EntityRef entity)
    {
        if (_parent.LegwearInstance)
        {
            foreach (Renderer skinnedMeshRenderer in _parent.LegwearInstance.GetComponentsInChildren<Renderer>())
                _parent.RemoveRenderer(skinnedMeshRenderer);

            Destroy(_parent.LegwearInstance.gameObject);
        }

        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(legwear.Template.Id);
        if (template && template.Instance)
        {
            _parent.SetLegwearInstance(Instantiate(template.Instance, transform).GetComponent<ParentClothing>());
            _parent.LegwearInstance.SetParent(_clothingParent);

            foreach (Renderer skinnedMeshRenderer in _parent.LegwearInstance.GetComponentsInChildren<Renderer>())
                _parent.AddRenderer(skinnedMeshRenderer);

            _parent.LegwearInstance.transform.GetChild(0).localScale = new Vector3(1, _parent.Direction, 1) * 100;
        }
    }

    public void SetEyes(EyesColorBinding eyes, EntityRef entity)
    {
        EyesAsset template = UnityDB.FindAsset<EyesAsset>(eyes.Eyes.Id);
        if (template && template.Texture)
        {
            _head.materials[2].SetTexture("_Base_Map", template.Texture);
        }

        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(eyes.Color.Id);
        if (color)
        {
            _head.materials[2].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
            _head.materials[2].SetFloat("_Smoothness", color.Settings_ColorPreset.Smoothness.AsFloat * 0.15f);
            _head.materials[2].SetFloat("_Metallic", color.Settings_ColorPreset.Metallic.AsFloat);
            _head.materials[2].SetFloat("_Fresnel_Strength", color.Settings_ColorPreset.FresnelStrength.AsFloat * 0.2f);
        }

        _parent.SetEyes(eyes);
    }

    public void SetHair(HairColorBinding hair, EntityRef entity)
    {
        if (_parent.HairInstance)
        {
            foreach (Renderer skinnedMeshRenderer in _parent.HairInstance.GetComponentsInChildren<Renderer>())
                _parent.RemoveRenderer(skinnedMeshRenderer);

            Destroy(_parent.HairInstance);
        }

        HairAsset template = UnityDB.FindAsset<HairAsset>(hair.Hair.Id);
        if (template && template.Hair)
        {
            _parent.SetHairInstance(Instantiate(template.Hair, _hair));

            foreach (Renderer skinnedMeshRenderer in _parent.HairInstance.GetComponentsInChildren<Renderer>())
                _parent.AddRenderer(skinnedMeshRenderer);
        }

        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(hair.Color.Id);
        if (color)
        {
            MeshRenderer meshRenderer = _parent.HairInstance.GetComponentInChildren<MeshRenderer>();

            meshRenderer.materials[0].SetColor("_Base_Color", color.Settings_ColorPreset.Color.ToColor());
            meshRenderer.materials[0].SetFloat("_Smoothness", color.Settings_ColorPreset.Smoothness.AsFloat * 0.15f);
            meshRenderer.materials[0].SetFloat("_Metallic", color.Settings_ColorPreset.Metallic.AsFloat);
            meshRenderer.materials[0].SetFloat("_Fresnel_Strength", color.Settings_ColorPreset.FresnelStrength.AsFloat * 0.2f);
        }
    }

    public void SetPlayerExpression(Expression expression)
    {
        _expression = expression;

        _expressionCurrentTime = 0;
        _expressionMaxLength = Random.Range(expression.Length.Min, expression.Length.Max);
    }

    public void SetHurtboxState(HurtboxSettings settings)
    {
        _parent.SetHurtboxSettings(settings);
    }

    private void UpdateHurtboxVisuals(float t)
    {
        foreach (Renderer renderer in _parent.MeshRenderers)
        {
            if (!renderer)
                continue;

            Material[] materials = renderer.materials;
            ApplyHurtboxEffect(materials, t);
        }
    }

    private void ApplyHurtboxEffect(Material[] materials, float t)
    {
        for (int i = 0; i < materials.Length; ++i)
        {
            if (!_parent.HurtboxSettings.CanBeInterrupted && !_parent.HurtboxSettings.CanBeDamaged && !_parent.HurtboxSettings.CanBeKnockedBack)
            {
                _parent.Intangible.Apply(materials[i], t);
            }
            else if (!_parent.HurtboxSettings.CanBeInterrupted && !_parent.HurtboxSettings.CanBeDamaged && _parent.HurtboxSettings.CanBeKnockedBack)
            {
                _parent.Invincible.Apply(materials[i], t);
            }
            else if (!_parent.HurtboxSettings.CanBeInterrupted && _parent.HurtboxSettings.CanBeDamaged && !_parent.HurtboxSettings.CanBeKnockedBack)
            {
                _parent.SuperArmor.Apply(materials[i], t);
            }
            else
            {
                _parent.Default.Apply(materials[i], t);
            }
        }
    }

    private void UpdateExpression()
    {
        if (_expressionCurrentTime >= _expressionMaxLength)
        {
            SetPlayerExpression(_defaultExpression);
        }

        EvaluateExpression(_expressionCurrentTime);

        _expressionCurrentTime += Time.deltaTime;
    }

    private void EvaluateExpression(float t)
    {
        if (_expression.OpenEyesAmount.HasValue)
        {
            _head.SetBlendShapeWeight(0, 100 - _expression.OpenEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
            _head.SetBlendShapeWeight(1, 100 - _expression.OpenEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
        }

        int offset = _parent.Avatar == _female ? 1 : 0;

        if (_expression.SmileAmount.HasValue)
            _head.SetBlendShapeWeight(2 + offset, _expression.SmileAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.OpenMouthAmount.HasValue)
            _head.SetBlendShapeWeight(3 + offset, _expression.OpenMouthAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.CuteMouthAmount.HasValue)
            _head.SetBlendShapeWeight(4 + offset, _expression.CuteMouthAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.AngryEyesAmount.HasValue)
        {
            _head.SetBlendShapeWeight(5 + offset, _expression.AngryEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
            _head.SetBlendShapeWeight(6 + offset, _expression.AngryEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
        }

        if (_expression.SadEyesAmount.HasValue)
        {
            _head.SetBlendShapeWeight(7 + offset, _expression.SadEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
            _head.SetBlendShapeWeight(8 + offset, _expression.SadEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
        }

        if (_expression.SadMouthAmount.HasValue)
            _head.SetBlendShapeWeight(9 + offset, _expression.SadMouthAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.AngryMouthAmount.HasValue)
            _head.SetBlendShapeWeight(10 + offset, _expression.AngryMouthAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.TongueStickAmount.HasValue)
            _head.SetBlendShapeWeight(11 + offset, _expression.TongueStickAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.HappyMouthAmount.HasValue)
            _head.SetBlendShapeWeight(12 + offset, _expression.HappyMouthAmount.Value.Evaluate(t / _expressionMaxLength) * 100);

        if (_expression.WideEyesAmount.HasValue)
        {
            _head.SetBlendShapeWeight(13 + offset, _expression.WideEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
            _head.SetBlendShapeWeight(14 + offset, _expression.WideEyesAmount.Value.Evaluate(t / _expressionMaxLength) * 100);
        }
    }

    private IEnumerator Blink()
    {
        for (int i = 0; i <= 6; ++i)
        {
            Blink(i * (1.0f / 6));
            yield return new WaitForEndOfFrame();
        }

        for (int i = 6; i >= 0; --i)
        {
            Blink(i * (1.0f / 6));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(Random.Range(1f, 4f));
        yield return Blink();
    }

    public void Blink(float weight)
    {
        _head.SetBlendShapeWeight(0, Mathf.Lerp(20, 100, weight));
        _head.SetBlendShapeWeight(1, Mathf.Lerp(20, 100, weight));
    }

    public void ChangeDirection(int direction)
    {
        transform.GetChild(0).localScale = new Vector3(1, direction, 1) * 100;

        if (_parent.HeadgearInstance)
            _parent.HeadgearInstance.transform.GetChild(0).localScale = new Vector3(1, direction, 1) * 100;
        
        if (_parent.ClothingInstance)
            _parent.ClothingInstance.transform.GetChild(0).localScale = new Vector3(1, direction, 1) * 100;
        
        if (_parent.LegwearInstance)
            _parent.LegwearInstance.transform.GetChild(0).localScale = new Vector3(1, direction, 1) * 100;

        _parent.SetDirection(direction);
    }
}
