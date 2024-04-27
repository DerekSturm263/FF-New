using System.Reflection;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class Any
    {
        public enum PropertyType
        {
            Object, UnityObject
        }
        [SerializeField] private PropertyType _type;

        [SerializeField] private string _typeName;
        public System.Type Type => System.Type.GetType(_typeName);

        [SerializeReference] private object _objValue;
        [SerializeField] private Object _unityObjValue;

        private Any(System.Type type, object value)
        {
            _typeName = type.AssemblyQualifiedName;

            if (Type.IsSubclassOf(typeof(Object)))
                _type = PropertyType.UnityObject;
            else
                _type = PropertyType.Object;

            if (_type == PropertyType.UnityObject)
            {
                _unityObjValue = (Object)value;
                _objValue = null;
            }
            else
            {
                _objValue = value;
                _unityObjValue = null;
            }
        }

        public T Get<T>()
        {
            if (_type == PropertyType.UnityObject)
                return (T)(object)GetUnityObjValue();
            else
                return GetObjValue<T>();
        }
        private T GetObjValue<T>()
        {
            try
            {
                if (_objValue is not null)
                    return (T)_objValue;
                else
                    return default;
            }
            catch
            {
                return default;
            }
        }
        private Object GetUnityObjValue()
        {
            if (_unityObjValue != null)
                return _unityObjValue;
            else
                return default;
        }

        public void Set<T>(T value)
        {
            if (typeof(T) != Type)
                throw new System.ArgumentException($"Given argument \"{typeof(T).Name}\" did not match the current type of the Any.");

            if (_type == PropertyType.UnityObject)
            {
                SetUnityObjValue((Object)(object)value);
                _objValue = null;
            }
            else
            {
                SetObjValue(value);
                _unityObjValue = null;
            }
        }
        private void SetObjValue<T>(T value) => _objValue = value;
        private void SetUnityObjValue(UnityEngine.Object value) => _unityObjValue = value;

        public bool IsNull
        {
            get
            {
                if (_type == PropertyType.UnityObject)
                    return _unityObjValue == null;
                else
                    return _objValue == null;
            }
        }

        public static object GetDefault(System.Type type)
        {
            System.Type thisType = typeof(Any);
            MethodInfo method = thisType.GetMethod("GetDefaultGeneric", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(type);
            object val = method.Invoke(null, null);

            return val;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static T GetDefaultGeneric<T>() => default;
#pragma warning restore IDE0051 // Remove unused private members

        public override bool Equals(object obj)
        {
            if (obj is Any any)
            {
                if (_type == PropertyType.UnityObject)
                    if (_unityObjValue != null)
                        return _unityObjValue.Equals(any._unityObjValue);
                    else
                        return obj is null;
                else
                    return Equals(_objValue, any._objValue);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => System.HashCode.Combine(_type, _typeName, _objValue, _unityObjValue);

        public static Any FromValue<T>(T value)
        {
            return new Any(typeof(T), value);
        }
    }
}
