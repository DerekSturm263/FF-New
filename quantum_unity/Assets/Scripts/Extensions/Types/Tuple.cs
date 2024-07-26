using System.Runtime.CompilerServices;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public struct Tuple<T1, T2> : ITuple
    {
        [SerializeField] private T1 _item1;
        public readonly T1 Item1 => _item1;

        [SerializeField] private T2 _item2;
        public readonly T2 Item2 => _item2;

        public Tuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        public object this[int index]
        {
            readonly get
            {
                return index switch
                {
                    0 => _item1,
                    1 => _item2,
                    _ => default
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _item1 = (T1)value;
                        break;

                    case 1:
                        _item2 = (T2)value;
                        break;
                }
            }
        }

        public readonly int Length => 2;
    }
}
