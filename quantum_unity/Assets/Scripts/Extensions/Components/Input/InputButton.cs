﻿using UnityEngine.InputSystem;
using UnityEngine;

namespace Extensions.Components.Input
{
    [CreateAssetMenu(fileName = "New Input Button", menuName = "Fusion Fighters/Input Button")]
    public class InputButton : ScriptableObject
    {
        [SerializeField] private Types.Dictionary<string, int> _ids;
        public int GetID(string controlName) => _ids[controlName];

        [SerializeField] private InputActionReference _action;
        public InputAction Action => _action ? _action.action : null;
    }
}
