using Extensions.Components.UI;
using Quantum;
using System;
using UnityEngine;

[Serializable]
public struct DisplayAllAvatarInfo
{
    [SerializeField] private DisplayAvatar _displayStyle;
    [SerializeField] private DisplayColorPreset _displayColor;

    public readonly void UpdateDisplay(AvatarColorBinding item)
    {
        FFAvatarAsset avatarStyle = UnityDB.FindAsset<FFAvatarAsset>(item.Avatar.Id);
        ColorPresetAsset avatarColor = UnityDB.FindAsset<ColorPresetAsset>(item.Color.Id);
    }
}

[Serializable]
public struct DisplayAllEyesInfo
{
    [SerializeField] private DisplayEyes _displayStyle;
    [SerializeField] private DisplayColorPreset _displayColor;

    public readonly void UpdateDisplay(EyesColorBinding item)
    {
        EyesAsset eyeStyle = UnityDB.FindAsset<EyesAsset>(item.Eyes.Id);
        ColorPresetAsset eyeColor = UnityDB.FindAsset<ColorPresetAsset>(item.Color.Id);
    }
}

[Serializable]
public struct DisplayAllHairInfo
{
    [SerializeField] private DisplayHair _displayStyle;
    [SerializeField] private DisplayColorPreset _displayColor;

    public readonly void UpdateDisplay(HairColorBinding item)
    {
        HairAsset hairStyle = UnityDB.FindAsset<HairAsset>(item.Hair.Id);
        ColorPresetAsset hairColor = UnityDB.FindAsset<ColorPresetAsset>(item.Color.Id);
    }
}

[Serializable]
public struct DisplayAllFrameInfo
{
    [SerializeField] private DisplayAllAvatarInfo _displayAvatar;
    [SerializeField] private DisplayAllEyesInfo _displayEyes;
    [SerializeField] private DisplayAllHairInfo _displayHair;
    [SerializeField] private DisplayVoice _displayVoice;

    public readonly void UpdateDisplay(FFFrame item)
    {
        VoiceAsset voice = UnityDB.FindAsset<VoiceAsset>(item.Voice.Id);

        _displayAvatar.UpdateDisplay(item.Avatar);
        _displayEyes.UpdateDisplay(item.Eyes);
        _displayHair.UpdateDisplay(item.Hair);
        _displayVoice.UpdateDisplay(voice);
    }
}

[Serializable]
public struct DisplayAllWeaponInfo
{
    [SerializeField] private DisplayWeaponTemplateInfo _displayTemplate;
    [SerializeField] private DisplayWeaponMaterialInfo _displayMaterial;
    [SerializeField] private DisplayWeaponEnhancerInfo _displayEnhancer;

    public readonly void UpdateDisplay(Weapon weapon)
    {
        WeaponTemplateAsset template = UnityDB.FindAsset<WeaponTemplateAsset>(weapon.Template.Id);
        WeaponMaterialAsset material = UnityDB.FindAsset<WeaponMaterialAsset>(weapon.Material.Id);
        WeaponEnhancerAsset enhancer = UnityDB.FindAsset<WeaponEnhancerAsset>(weapon.Enhancer.Id);

        _displayTemplate.UpdateDisplay(template);
        _displayMaterial.UpdateDisplay(material);
        _displayEnhancer.UpdateDisplay(enhancer);
    }
}

[Serializable]
public struct DisplayAllSubInfo
{
    [SerializeField] private DisplaySubTemplateInfo _displayTemplate;
    [SerializeField] private DisplaySubEnhancerInfo _displayEnhancer;

    public readonly void UpdateDisplay(Sub item)
    {
        SubTemplateAsset template = UnityDB.FindAsset<SubTemplateAsset>(item.Template.Id);
        SubEnhancerAsset enhancer = UnityDB.FindAsset<SubEnhancerAsset>(item.Enhancer.Id);

        _displayTemplate.UpdateDisplay(template);
        _displayEnhancer.UpdateDisplay(enhancer);
    }
}

[Serializable]
public struct DisplayAllGearInfo
{
    [SerializeField] private DisplayAllWeaponInfo _displayMainWeapon;
    [SerializeField] private DisplayAllWeaponInfo _displayAltWeapon;
    [SerializeField] private DisplayAllSubInfo _displaySub;
    [SerializeField] private DisplayBadge _displayBadge;
    [SerializeField] private DisplayUltimate _displayUltimate;

    public readonly void UpdateDisplay(Gear item)
    {
        BadgeAsset badge = UnityDB.FindAsset<BadgeAsset>(item.Badge.Id);
        UltimateAsset ultimate = UnityDB.FindAsset<UltimateAsset>(item.Ultimate.Id);

        _displayMainWeapon.UpdateDisplay(item.MainWeapon);
        _displayAltWeapon.UpdateDisplay(item.AltWeapon);
        _displaySub.UpdateDisplay(item.SubWeapon);
        _displayBadge.UpdateDisplay(badge);
        _displayUltimate.UpdateDisplay(ultimate);
    }
}

[Serializable]
public struct DisplayAllApparelInfo
{
    [SerializeField] private DisplayApparelTemplateInfo _displayTemplate;
    [SerializeField] private DisplayApparelModifierInfo _displayModifier1;
    [SerializeField] private DisplayApparelModifierInfo _displayModifier2;
    [SerializeField] private DisplayApparelModifierInfo _displayModifier3;
    [SerializeField] private DisplayColorPreset _displayColor;

    public readonly void UpdateDisplay(Apparel item)
    {
        ApparelTemplateAsset template = UnityDB.FindAsset<ApparelTemplateAsset>(item.Template.Id);
        ApparelModifierAsset modifier1 = UnityDB.FindAsset<ApparelModifierAsset>(item.Modifiers.Modifier1.Id);
        ApparelModifierAsset modifier2 = UnityDB.FindAsset<ApparelModifierAsset>(item.Modifiers.Modifier2.Id);
        ApparelModifierAsset modifier3 = UnityDB.FindAsset<ApparelModifierAsset>(item.Modifiers.Modifier3.Id);
        ColorPresetAsset color = UnityDB.FindAsset<ColorPresetAsset>(item.Color.Id);

        _displayTemplate.UpdateDisplay(template);
        _displayModifier1.UpdateDisplay(modifier1);
        _displayModifier2.UpdateDisplay(modifier2);
        _displayModifier3.UpdateDisplay(modifier3);
        _displayModifier1.UpdateDisplay(modifier1);
    }
}

[Serializable]
public struct DisplayAllOutfitInfo
{
    [SerializeField] private DisplayAllApparelInfo _displayHeadgear;
    [SerializeField] private DisplayAllApparelInfo _displayClothing;
    [SerializeField] private DisplayAllApparelInfo _displayLegwear;

    public readonly void UpdateDisplay(Outfit item)
    {
        _displayHeadgear.UpdateDisplay(item.Headgear);
        _displayClothing.UpdateDisplay(item.Clothing);
        _displayLegwear.UpdateDisplay(item.Legwear);
    }
}

[Serializable]
public struct DisplayAllEmoteInfo
{
    [SerializeField] private DisplayEmote _displayEmote;
    [SerializeField] private DisplayMessagePreset _displayMessage;

    public readonly void UpdateDisplay(EmoteMessageBinding item)
    {
        EmoteAsset emote = UnityDB.FindAsset<EmoteAsset>(item.Emote.Id);
        MessagePresetAsset message = UnityDB.FindAsset<MessagePresetAsset>(item.Message.Id);

        _displayEmote.UpdateDisplay(emote);
        _displayMessage.UpdateDisplay(message);
    }
}

[Serializable]
public struct DisplayAllEmotesInfo
{
    [SerializeField] private DisplayAllEmoteInfo _displayUp;
    [SerializeField] private DisplayAllEmoteInfo _displayDown;
    [SerializeField] private DisplayAllEmoteInfo _displayLeft;
    [SerializeField] private DisplayAllEmoteInfo _displayRight;

    public readonly void UpdateDisplay(DirectionalEmote item)
    {
        _displayUp.UpdateDisplay(item.Up);
        _displayDown.UpdateDisplay(item.Down);
        _displayLeft.UpdateDisplay(item.Left);
        _displayRight.UpdateDisplay(item.Right);
    }
}

public class DisplayAllBuildInfo : DisplayBase
{
    [SerializeField] private DisplayAllFrameInfo _displayFrame;
    [SerializeField] private DisplayAllGearInfo _displayGear;
    [SerializeField] private DisplayAllOutfitInfo _displayOutfit;
    [SerializeField] private DisplayAllEmotesInfo _displayEmotes;

    public void UpdateDisplay(Build item)
    {
        _displayFrame.UpdateDisplay(item.Frame);
        _displayGear.UpdateDisplay(item.Gear);
        _displayOutfit.UpdateDisplay(item.Outfit);
        _displayEmotes.UpdateDisplay(item.Emotes);
    }

    public override void UpdateDisplayOnEnable() => UpdateDisplay(GetValue());

    protected Build GetValue() => default;
}
