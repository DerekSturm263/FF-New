using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Extensions.Components.Input
{
    public class InputMapperController : Miscellaneous.Controller<InputMapperController>
    {
        [SerializeField] private Types.Dictionary<string, TMPro.TMP_SpriteAsset> _controlSchemesToSpriteAssets;
        public Types.Dictionary<string, TMPro.TMP_SpriteAsset> ControlSchemesToSpriteAssets => _controlSchemesToSpriteAssets;

        private InputDevice _currentDevice;
        public InputDevice CurrentDevice => _currentDevice;

        private IDisposable _event;

        public override void Initialize()
        {
            base.Initialize();
            
            _event = InputSystem.onAnyButtonPress.Call(SetAllInputDevices);

            Application.quitting += Shutdown;
        }

        public override void Shutdown()
        {
            _event.Dispose();
            Application.quitting -= Shutdown;

            base.Shutdown();
        }

        public void SetAllInputDevices(InputControl action)
        {
            _currentDevice = action.device;

            foreach (ButtonPrompt prompt in FindObjectsByType<ButtonPrompt>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                prompt.DisplayInput(action.device);
            }
        }
    }
}
