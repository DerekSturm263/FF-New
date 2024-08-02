using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Miscellaneous
{
    public class ScriptableEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onAwake;
        public void SubscribeOnAwake(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onEnable;
        public void SubscribeOnEnable(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onStart;
        public void SubscribeOnStart(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onUpdate;
        public void SubscribeOnUpdate(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onFixedUpdate;
        public void SubscribeOnFixedUpdate(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onDisable;
        public void SubscribeOnDisable(UnityAction action) => _onAwake.AddListener(action);

        [SerializeField] private UnityEvent _onDestroy;
        public void SubscribeOnDestroy(UnityAction action) => _onAwake.AddListener(action);

        private void Awake() => _onAwake?.Invoke();
        private void OnEnable() => _onEnable?.Invoke();
        private void Start() => _onStart?.Invoke();
        private void Update() => _onUpdate?.Invoke();
        private void FixedUpdate() => _onFixedUpdate?.Invoke();
        private void OnDisable() => _onDisable?.Invoke();
        private void OnDestroy() => _onDestroy?.Invoke();
    }
}
