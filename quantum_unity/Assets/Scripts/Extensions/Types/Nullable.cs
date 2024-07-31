using System;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public struct Nullable<T> where T : struct
    {
        [SerializeField] private T _nonNullValue;
        [SerializeField] private bool _hasValue;

        public readonly bool HasValue => _hasValue;
        public readonly T Value => _nonNullValue;

        public Nullable(T? value)
        {
            if (value is not null)
                _nonNullValue = value.Value;
            else
                _nonNullValue = default;

            _hasValue = value.HasValue;
        }

        public readonly T GetValueOrDefault()
        {
            if (_hasValue)
                return _nonNullValue;
            else
                return default;
        }

        public readonly T GetValueOrDefault(T defaultValue)
        {
            if (_hasValue)
                return _nonNullValue;
            else
                return defaultValue;
        }

        public override readonly bool Equals(object other)
        {
            if (other is null || other is not Nullable<T>)
                return false;

            return _nonNullValue.Equals(((Nullable<T>)other)._nonNullValue) && _hasValue.Equals(((Nullable<T>)other)._hasValue);
        }

        public override readonly int GetHashCode() => HashCode.Combine(_nonNullValue, _hasValue);

        public override readonly string ToString() => _nonNullValue.ToString();

        public static implicit operator Nullable<T>(T value) => new(value);
        public static explicit operator T(Nullable<T> value) => value.GetValueOrDefault();
    }
}
