using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{
    public static partial class DeterministicCommandSetup
    {
        static partial void AddCommandFactoriesUser(ICollection<IDeterministicCommandFactory> factories, RuntimeConfig gameConfig, SimulationConfig simulationConfig)
        {
            // user commands go here
            factories.Add(new CommandSetBuild());
            factories.Add(new CommandSetStage());
            factories.Add(new CommandSetRuleset());
            factories.Add(new CommandSpawnAI());
            factories.Add(new CommandDespawnPlayer());
            factories.Add(new CommandResetMatch());
            factories.Add(new CommandSetAltWeapon());
            factories.Add(new CommandSetAvatar());
            factories.Add(new CommandSetBadge());
            factories.Add(new CommandSetBuild());
            factories.Add(new CommandSetClothing());
            factories.Add(new CommandSetEmoteDown());
            factories.Add(new CommandSetEmoteLeft());
            factories.Add(new CommandSetEmoteRight());
            factories.Add(new CommandSetEmoteUp());
            factories.Add(new CommandSetEyes());
            factories.Add(new CommandSetHair());
            factories.Add(new CommandSetHeadgear());
            factories.Add(new CommandSetLegwear());
            factories.Add(new CommandSetMainWeapon());
            factories.Add(new CommandSetSub());
            factories.Add(new CommandSetUltimate());
            factories.Add(new CommandSetVoice());
        }
    }
}
