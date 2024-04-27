using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public struct Positional<T>
    {
        [SerializeField] private T _value;
        public readonly T Value => _value;

        [SerializeField] private Vector2 _position;
        public readonly Vector2 Position => _position;

        public Positional(T value, Vector2 position)
        {
            _value = value;
            _position = position;
        }
    }
}
