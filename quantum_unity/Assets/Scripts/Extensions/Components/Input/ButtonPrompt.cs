﻿using TMPro;
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

        private InputDevice _device;

        private void Awake()
        {
            Setup();
            DisplayInput(InputMapperController.Instance.CurrentDevice);
        }

        public void Setup()
        {
            GetComponent<Button>().onClick.AddListener(_inputEvent.Invoke);
        }

        public void DisplayInput(InputDevice device)
        {
            if (device == _device)
                return;

            _device = device;
            DisplayInputs(device.displayName);
        }

        private void DisplayInputs(string deviceName)
        {
            if (!_text)
                _text = GetComponentInChildren<TMPro.TMP_Text>();

            TMP_SpriteAsset spriteAsset = InputMapperController.Instance.GetSpriteAsset(deviceName);
            if (!spriteAsset)
                return;

            _text.spriteAsset = spriteAsset;

            if (_inputEvent.Button2Optional)
                _text.SetText(string.Format(_format, $"<sprite name=\"{_inputEvent.Button.GetID(deviceName)}\"><sprite name=\"{_inputEvent.Button2Optional.GetID(deviceName)}\">"));
            else
                _text.SetText(string.Format(_format, $"<sprite name=\"{_inputEvent.Button.GetID(deviceName)}\">"));
        }
    }
}
