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

        [SerializeField] private EntityView _timer;

        [SerializeField] private string _format = "hh':'mm':'ss";
        [SerializeField] private bool _trimStart;
        [SerializeField] private bool _trimEnd;

        [SerializeField] private Types.Dictionary<int, UnityEvent<int>> _tickEvents;
        [SerializeField] private UnityEvent _onTick;
        public void InvokeTickEvent(int index)
        {
            if (_tickEvents.TryGetValue(index, out UnityEvent<int> unityEvent))
                unityEvent.Invoke(index);

            _onTick.Invoke();
        }

        private TMPro.TMP_Text _text;

        protected override void Awake()
        {
            _text = GetComponent<TMPro.TMP_Text>();

            QuantumEvent.Subscribe<EventOnTimerTick>(listener: this, handler: e =>
            {
                if (e.Caller != _timer.EntityRef)
                    return;

                UpdateTimer(e.Time);
            });
        }

        private void UpdateTimer(int time)
        {
            _time = System.TimeSpan.FromSeconds(time);

            string text = new(_time.ToString(_format));
            if (_trimStart)
                text = text.TrimStart('0');
            if (_trimEnd)
                text = text.TrimEnd('0');

            _text.SetText(text);

            InvokeTickEvent(time);
        }
    }
}
