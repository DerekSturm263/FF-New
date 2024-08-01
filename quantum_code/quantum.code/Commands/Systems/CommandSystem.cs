namespace Quantum
{
    public unsafe class CommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int i = 0; i < f.PlayerCount; i++)
            {
                var command = f.GetPlayerCommand(i);

                if (command is CommandSetBuild commandSetBuild)
                    commandSetBuild.Execute(f);
                else if (command is CommandSetStage commandSetStage)
                    commandSetStage.Execute(f);
                else if (command is CommandSetRuleset commandSetRuleset)
                    commandSetRuleset.Execute(f);
                else if (command is CommandSpawnAI commandSpawnAI)
                    commandSpawnAI.Execute(f);
                else if (command is CommandDespawnPlayer commandDespawnPlayer)
                    commandDespawnPlayer.Execute(f);
                else if (command is CommandResetMatch commandResetMatch)
                    commandResetMatch.Execute(f);
                else if (command is CommandSetupMatch commandSetupMatch)
                    commandSetupMatch.Execute(f);
                else if (command is CommandPlayerApplyProfile commandPlayerApplyProfile)
                    commandPlayerApplyProfile.Execute(f);
                else if (command is CommandSetAltWeapon commandSetAltWeapon)
                    commandSetAltWeapon.Execute(f);
                else if (command is CommandSetAvatar commandSetAvatar)
                    commandSetAvatar.Execute(f);
                else if (command is CommandSetBadge commandSetBadge)
                    commandSetBadge.Execute(f);
                else if (command is CommandSetClothing commandSetClothing)
                    commandSetClothing.Execute(f);
                else if (command is CommandSetEmoteDown commandSetEmoteDown)
                    commandSetEmoteDown.Execute(f);
                else if (command is CommandSetEmoteLeft commandSetEmoteLeft)
                    commandSetEmoteLeft.Execute(f);
                else if (command is CommandSetEmoteRight commandSetEmoteRight)
                    commandSetEmoteRight.Execute(f);
                else if (command is CommandSetEmoteUp commandSetEmoteUp)
                    commandSetEmoteUp.Execute(f);
                else if (command is CommandSetEyes commandSetEyes)
                    commandSetEyes.Execute(f);
                else if (command is CommandSetHair commandSetHair)
                    commandSetHair.Execute(f);
                else if (command is CommandSetHeadgear commandSetHeadgear)
                    commandSetHeadgear.Execute(f);
                else if (command is CommandSetLegwear commandSetLegwear)
                    commandSetLegwear.Execute(f);
                else if (command is CommandSetMainWeapon commandSetMainWeapon)
                    commandSetMainWeapon.Execute(f);
                else if (command is CommandSetSub commandSetSub)
                    commandSetSub.Execute(f);
                else if (command is CommandSetUltimate commandSetUltimate)
                    commandSetUltimate.Execute(f);
                else if (command is CommandSetVoice commandSetVoice)
                    commandSetVoice.Execute(f);
                else if (command is CommandSetAvatarColor commandSetAvatarColor)
                    commandSetAvatarColor.Execute(f);
                else if (command is CommandSetHairColor commandSetHairColor)
                    commandSetHairColor.Execute(f);
                else if (command is CommandSetEyeColor commandSetEyeColor)
                    commandSetEyeColor.Execute(f);
                else if (command is CommandPause commandPause)
                    commandPause.Execute(f);
                else if (command is CommandPlay commandPlay)
                    commandPlay.Execute(f);
                else if (command is CommandSetTimeScale commandSetTimeScale)
                    commandSetTimeScale.Execute(f);
            }
        }
    }
}
