using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Extensions.Components.Input
{
    [RequireComponent(typeof(Button))]
    public class ButtonPrompt : MonoBehaviour
    {
        [SerializeField][Multiline] private string _format = "{0}";
        [SerializeField] private InputEvent _inputEvent;

        private TMPro.TMP_Text _text;
        private InputDevice _lastDevice;

        private void Awake() => Setup();

        public void Setup()
        {
            GetComponent<Button>().onClick.AddListener(_inputEvent.Invoke);
        }

        public void DisplayInputs(InputDevice device)
        {
            if (device == _lastDevice)
                return;

            if (!_text)
                _text = GetComponentInChildren<TMPro.TMP_Text>();

            if (!InputMapperController.Instance.ControlSchemesToSpriteAssets.ContainsKey(device.name))
                return;

            _text.spriteAsset = InputMapperController.Instance.ControlSchemesToSpriteAssets[device.name];
            _text.SetText(string.Format(_format, $"<sprite={_inputEvent.Button.GetID(device.name)}>"));

            _lastDevice = device;
        }
    }
}
