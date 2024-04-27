using UnityEngine;

namespace Extensions.Types
{
    [System.Serializable]
    public struct SizeVariants<T>
    {
        [SerializeField] private T _small;
        public readonly T Small => _small;

        [SerializeField] private T _medium;
        public readonly T Medium => _medium;

        [SerializeField] private T _large;
        public readonly T Large => _large;

        public SizeVariants(T small, T medium, T large)
        {
            _small = small;
            _medium = medium;
            _large = large;
        }

        public readonly T GetFromAmount(float amount)
        {
            if (amount < 0.33f)
                return _small;
            else if (amount < 0.66f)
                return _medium;
            else
                return _large;
        }
    }
}
