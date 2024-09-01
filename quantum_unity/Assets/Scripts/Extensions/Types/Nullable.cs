using System;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class Nullable<T>
    {
        [SerializeField] private T _nonNullValue;
        [SerializeField] private bool _hasValue;

        public bool HasValue => _hasValue;
        public T Value => _nonNullValue;

        public Nullable(T value)
        {
            _nonNullValue = value;
            _hasValue = true;
        }

        public Nullable()
        {
            _nonNullValue = default;
            _hasValue = false;
        }

        public T GetValueOrDefault()
        {
            if (_hasValue)
                return _nonNullValue;
            else
                return default;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            if (_hasValue)
                return _nonNullValue;
            else
                return defaultValue;
        }

        public override bool Equals(object other)
        {
            if (other is null || other is not Nullable<T>)
                return false;

            return _nonNullValue.Equals(((Nullable<T>)other)._nonNullValue) && _hasValue.Equals(((Nullable<T>)other)._hasValue);
        }

        public override int GetHashCode() => HashCode.Combine(_nonNullValue, _hasValue);

        public override string ToString() => _nonNullValue.ToString();

        public static implicit operator Nullable<T>(T value) => new(value);
        public static explicit operator T(Nullable<T> value) => value.GetValueOrDefault();
    }
}
