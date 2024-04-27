using Quantum;
using System.Collections.Generic;
using UnityEngine;

namespace GameResources.Challenge
{
    public class ChallengeController : Extensions.Components.Miscellaneous.Controller<ChallengeController>
    {
        [SerializeField] private List<AssetRef> _challenges;

        public override void Initialize()
        {
            // Make sure to call the base class's initialize.
            base.Initialize();

            // Iterate through every Stat.
            foreach (AssetRef asset in _challenges)
            {

            }
        }
    }
}
