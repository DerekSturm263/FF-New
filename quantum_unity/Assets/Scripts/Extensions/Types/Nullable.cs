using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class Nullable<T> : ISerializationCallbackReceiver where T : struct
    {
        [SerializeField] private T _nonNullValue;
        [SerializeField] private bool _hasValue;

        private T? _value;

        public bool HasValue => _value.HasValue;
        public T Value => _value.Value;

        public Nullable(T value)
        {
            _value = value;
        }

        public override bool Equals(object other) => _value.Equals(other as Nullable<T>);
        public override int GetHashCode() => _value.GetHashCode();
        public T GetValueOrDefault() => _value.GetValueOrDefault();
        public T GetValueOrDefault(T defaultValue) => _value.GetValueOrDefault(defaultValue);
        public override string ToString() => _value.ToString();

        public static implicit operator Nullable<T>(T value) => new(value);
        public static explicit operator T(Nullable<T> value) => value.GetValueOrDefault();

        public void OnBeforeSerialize()
        {
            _nonNullValue = _value.GetValueOrDefault();
            _hasValue = _value.HasValue;
        }

        public void OnAfterDeserialize()
        {
            if (_hasValue)
                _value = _nonNullValue;
            else
                _value = null;
        }
    }
}
