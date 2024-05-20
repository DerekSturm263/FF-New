using Quantum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Timer", 79)]
    [RequireComponent(typeof(TMPro.TMP_Text))]
    [DisallowMultipleComponent]
    public class TimerDisplay : UIBehaviour
    {
        private System.TimeSpan _time;

        [SerializeField] private string _format = "hh':'mm':'ss";
        [SerializeField] private bool _trimStart;
        [SerializeField] private bool _trimEnd;
        [SerializeField] private bool _useBeginningCountdown;

        [SerializeField] private Types.Dictionary<int, UnityEvent<int>> _tickEvents;
        [SerializeField] private UnityEvent<string> _onTick;

        public void InvokeTickEvent(int time)
        {
            if (_tickEvents.TryGetValue(time, out UnityEvent<int> unityEvent))
                unityEvent.Invoke(time);
        }

        protected override void Awake()
        {
            if (_useBeginningCountdown)
                QuantumEvent.Subscribe<EventOnBeginningCountdown>(listener: this, handler: e => UpdateTimer(e.Time));
            else
                QuantumEvent.Subscribe<EventOnTimerTick>(listener: this, handler: e => UpdateTimer(e.Time));
        }

        private void UpdateTimer(int time)
        {
            _time = System.TimeSpan.FromSeconds(time);

            string text = new(_time.ToString(_format));
            if (_trimStart)
                text = text.TrimStart('0');
            if (_trimEnd)
                text = text.TrimEnd('0');

            _onTick.Invoke(text);
            InvokeTickEvent(time);
        }
    }
}
