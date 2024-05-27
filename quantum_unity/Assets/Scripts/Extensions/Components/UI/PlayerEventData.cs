using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEngine.EventSystems
{
    public class PlayerEventData : BaseEventData
    {
        public int PlayerNum;

        public PlayerEventData(EventSystem eventSystem) : base(eventSystem)
        {
            PlayerNum = -1;
        }
    }
}