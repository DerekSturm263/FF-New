using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Extensions.Components.Input
{
    public class InputMapperController : Miscellaneous.Controller<InputMapperController>
    {
        [SerializeField] private Types.Dictionary<string, TMPro.TMP_SpriteAsset> _controlSchemesToSpriteAssets;
        public TMPro.TMP_SpriteAsset GetSpriteAsset(string controlScheme) => _controlSchemesToSpriteAssets[controlScheme];

        [SerializeField] private Types.Dictionary<string, Sprite> _controlSchemesToIcons;
        public Sprite GetIcon(string controlScheme) => _controlSchemesToIcons[controlScheme];

        private InputDevice _currentDevice;
        public InputDevice CurrentDevice => _currentDevice;

        private IDisposable _event;

        public override void Initialize()
        {
            base.Initialize();

            _currentDevice ??= InputSystem.devices.First(item => _controlSchemesToSpriteAssets.ContainsKey(item.displayName));
            _event = InputSystem.onAnyButtonPress.Call(SetAllInputDevices);

            Application.quitting += Shutdown;
        }

        public override void Shutdown()
        {
            _event.Dispose();
            Application.quitting -= Shutdown;

            _currentDevice = null;

            base.Shutdown();
        }

        public void SetAllInputDevices(InputControl action)
        {
            if (!_controlSchemesToSpriteAssets.ContainsKey(action.device.displayName))
                return;

            _currentDevice = action.device;

            foreach (ButtonPrompt prompt in FindObjectsByType<ButtonPrompt>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                prompt.DisplayInput(action.device);
            }
        }
    }
}
