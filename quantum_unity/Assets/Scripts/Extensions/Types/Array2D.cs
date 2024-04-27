using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class Array2D<T>
    {
        [System.Serializable]
        internal struct Array2DInternal
        {
            [SerializeField] internal List<T> _array;
        }

        [SerializeField] private List<Array2DInternal> _array;
        public T[,] Array
        {
            get
            {
                if (_array.Count == 0 || _array[0]._array.Count == 0)
                    return new T[0, 0];

                T[,] array = new T[_array.Count, _array[0]._array.Count];
                for (int i = 0; i < _array.Count; ++i)
                {
                    for (int j = 0; j < _array[i]._array.Count; ++j)
                    {
                        array[i, j] = _array[i]._array[j];
                    }
                }

                return array;
            }
        }

        public T this[int x, int y]
        {
            get => _array[x]._array[y];
            set => _array[x]._array[y] = value;
        }

        public Vector2Int GetSize => new(_array.Count, _array[0]._array.Count);

        public Array2D(int rowCount, int columnCount)
        {
            _array = new List<Array2DInternal>(rowCount);
            for (int i = 0; i < rowCount; ++i)
            {
                Array2DInternal internalArray = new()
                {
                    _array = new(columnCount)
                };
                for (int j = 0; j < columnCount; ++j)
                {
                    internalArray._array.Add(default);
                }

                _array.Add(internalArray);
            }
        }

        public Array2D(T[,] initial)
        {
            for (int i = 0; i < initial.GetLength(0); ++i)
            {
                for (int j = 0; j < initial.GetLength(1); ++j)
                {
                    _array[i]._array[j] = initial[i, j];
                }
            }
        }
    }
}
