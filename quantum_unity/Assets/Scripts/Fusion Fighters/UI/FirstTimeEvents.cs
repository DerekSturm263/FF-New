using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Miscellaneous
{
    public class FirstTimeEvents : MonoBehaviour
    {
        [SerializeField] private string _identifier;
        [SerializeField] private UnityEvent _events;

        private void OnEnable()
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
