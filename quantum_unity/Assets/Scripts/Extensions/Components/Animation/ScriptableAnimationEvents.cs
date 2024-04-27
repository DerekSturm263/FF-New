using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Animation
{
    public class ScriptableAnimationEvents : StateMachineBehaviour
    {
        [SerializeField] private UnityEvent<Animator, AnimatorStateInfo, int> _onStateEnter;
        [SerializeField] private UnityEvent<Animator, AnimatorStateInfo, int> _onStateUpdate;
        [SerializeField] private UnityEvent<Animator, AnimatorStateInfo, int> _onStateExit;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateEnter?.Invoke(animator, stateInfo, layerIndex);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateUpdate?.Invoke(animator, stateInfo, layerIndex);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onStateExit?.Invoke(animator, stateInfo, layerIndex);
        }
    }
}
