using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public struct RandomList<T>
    {
        [SerializeField] private List<T> _list;

        public readonly T Random => _list[UnityEngine.Random.Range(0, _list.Count)];

        public readonly int Count => ((IReadOnlyCollection<T>)_list).Count;
    }
}
