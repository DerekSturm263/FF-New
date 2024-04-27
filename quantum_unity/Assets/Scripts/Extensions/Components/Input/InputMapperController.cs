using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Extensions.Components.Input
{
    public class InputMapperController : Miscellaneous.Controller<InputMapperController>
    {
        [SerializeField] private Types.Dictionary<string, TMPro.TMP_SpriteAsset> _controlSchemesToSpriteAssets;
        public Types.Dictionary<string, TMPro.TMP_SpriteAsset> ControlSchemesToSpriteAssets => _controlSchemesToSpriteAssets;

        protected ButtonPrompt[] _allButtonPrompts;

        private InputDevice _lastDevice;

        public override void Initialize()
        {
            base.Initialize();

            _allButtonPrompts = FindObjectsByType<ButtonPrompt>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            InputSystem.onAnyButtonPress.Call(ctx => SetAllInputDevices(ctx.device));

            SetAllInputDevicesDefault();
        }

        public void SetAllInputDevicesDefault() => SetAllInputDevices(_lastDevice ?? InputSystem.devices[0]);

        public void SetAllInputDevices(InputDevice device)
        {
            foreach (ButtonPrompt prompt in _allButtonPrompts)
            {
                if (prompt)
                    prompt.DisplayInputs(device);
            }

            _lastDevice = device;
        }
    }
}
