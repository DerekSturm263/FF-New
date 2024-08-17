using UnityEngine.InputSystem;
using UnityEngine;

namespace Extensions.Components.Input
{
    [CreateAssetMenu(fileName = "New Input Button", menuName = "Fusion Fighters/Input Button")]
    public class InputButton : ScriptableObject
    {
        [SerializeField] private Types.Dictionary<string, Sprite> _idsToIcons;
        public string GetID(string controlName)
        {
            if (_idsToIcons.TryGetValue(controlName, out Sprite sprite) && sprite)
                return sprite.name;

            return string.Empty;
        }
        [SerializeField] private Types.Dictionary<string, Sprite> _idsToIconsPositive;
        public string GetIDPositive(string controlName)
        {
            if (_idsToIconsPositive.TryGetValue(controlName, out Sprite sprite) && sprite)
                return sprite.name;

            return string.Empty;
        }

        [SerializeField] private InputAction _action;
        public InputAction Action => _action;
    }
}
