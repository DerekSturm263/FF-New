using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public class ReferenceList<T> : IEnumerable<T>, IEnumerable
    {
        [SerializeReference] private List<T> _listInternal;

        public ReferenceList(List<T> listInternal)
        {
            _listInternal = listInternal;
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_listInternal).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_listInternal).GetEnumerator();

        public static implicit operator List<T>(ReferenceList<T> rhs) => rhs._listInternal;
    }
}
