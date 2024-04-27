using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class Tuple<T1, T2> : IStructuralComparable, IStructuralEquatable, System.IComparable, ITuple, ISerializationCallbackReceiver
    {
        [SerializeField] private T1 _item1;
        public T1 Item1 => _item1;

        [SerializeField] private T2 _item2;
        public T2 Item2 => _item2;

        private System.Tuple<T1, T2> _value = new(default, default);

        public Tuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;

            _value = new(item1, item2);
        }

        public object this[int index] => ((ITuple)_value)[index];
        public int Length => ((ITuple)_value).Length;

        public int CompareTo(object other, IComparer comparer) => ((IStructuralComparable)_value).CompareTo(other, comparer);
        public int CompareTo(object obj) => ((System.IComparable)_value).CompareTo(obj);
        public bool Equals(object other, IEqualityComparer comparer) => ((IStructuralEquatable)_value).Equals(other, comparer);
        public int GetHashCode(IEqualityComparer comparer) => ((IStructuralEquatable)_value).GetHashCode(comparer);

        public void OnBeforeSerialize()
        {
            _value ??= new(default, default);

            _item1 = _value.Item1;
            _item2 = _value.Item2;
        }

        public void OnAfterDeserialize()
        {
            _value = new(_item1, _item2);
        }
    }
}
