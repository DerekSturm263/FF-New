using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Types
{
    [System.Serializable]
    public class ReturningUnityEvent<TOut>
    {
        public class Resolver
        {
            private TOut _returnValue = default;
            public TOut ReturnValue => _returnValue;

            public void SetReturnValue(TOut returnValue) => _returnValue = returnValue;
        }

        [SerializeField] private UnityEvent<Resolver> _event;

        public TOut Invoke()
        {
            Resolver resolver = new();
            _event.Invoke(resolver);

            return resolver.ReturnValue;
        }
    }

    [System.Serializable]
    public class ReturningUnityEvent<TIn, TOut>
    {
        public class Resolver
        {
            private TOut _returnValue = default;
            internal TOut ReturnValue => _returnValue;

            public void SetReturnValue(TOut returnValue) => _returnValue = returnValue;
        }

        [SerializeField] private UnityEvent<TIn, Resolver> _event;

        public TOut Invoke(TIn arg0)
        {
            Resolver resolver = new();
            _event.Invoke(arg0, resolver);

            return resolver.ReturnValue;
        }
    }

    [System.Serializable]
    public class ReturningUnityEvent<TIn0, TIn1, TOut>
    {
        public class Resolver
        {
            private TOut _returnValue = default;
            internal TOut ReturnValue => _returnValue;

            public void SetReturnValue(TOut returnValue) => _returnValue = returnValue;
        }

        [SerializeField] private UnityEvent<TIn0, TIn1, Resolver> _event;

        public TOut Invoke(TIn0 arg0, TIn1 arg1)
        {
            Resolver resolver = new();
            _event.Invoke(arg0, arg1, resolver);

            return resolver.ReturnValue;
        }
    }
}
