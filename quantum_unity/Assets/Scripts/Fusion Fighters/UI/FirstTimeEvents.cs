using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Miscellaneous
{
    public class FirstTimeEvents : MonoBehaviour
    {
        [SerializeField] private string _identifier;
        [SerializeField] private float _delay = 0.1f;
        [SerializeField] private UnityEvent _events;

        private void OnEnable() => Invoke(nameof(Enable), _delay);

        private void Enable()
        {
            SettingsController settings = (SettingsController.Instance as SettingsController);

            if (!settings.Settings.VisitedScenes.Contains(_identifier))
            {
                _events?.Invoke();
                settings.Settings.VisitedScenes.Add(_identifier);
            }
        }
    }
}
