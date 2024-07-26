using Quantum;

namespace UnityEngine.EventSystems
{
    public class PlayerEventData : BaseEventData
    {
        public FighterIndex PlayerIndex;

        public PlayerEventData(EventSystem eventSystem) : base(eventSystem)
        {
            PlayerIndex = FighterIndex.Invalid;
        }
    }
}