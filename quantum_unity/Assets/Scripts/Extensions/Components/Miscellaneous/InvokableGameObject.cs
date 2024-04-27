using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Miscellaneous
{
    public class InvokableGameObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event;
        public void Invoke() => _event.Invoke();
    }
}
