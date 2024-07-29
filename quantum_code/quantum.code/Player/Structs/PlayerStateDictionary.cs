using Quantum;
using System.Collections.Generic;

namespace Quantum
{
    public class PlayerStateDictionary : Dictionary<States, PlayerState>
    {
        public PlayerStateDictionary(params PlayerState[] states)
        {
            foreach (var state in states)
            {
                Add(state.GetState().Item1, state);
            }
        }
    }
}
