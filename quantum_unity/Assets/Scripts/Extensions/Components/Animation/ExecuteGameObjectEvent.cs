using UnityEngine;
using UnityEngine.Events;

namespace Extensions.Components.Animation
{
    [System.Obsolete("This will get phased out soon. This is a reminder to phase this out.")]
    // TODO: MERGE WITH SCRIPTABLE ANIMATION EVENTS
    public class ExecuteGameObjectEvent : StateMachineBehaviour
    {
        [System.Flags]
        public enum When
        {
            Enter = 1 << 0,
            Update = 1 << 1,
            Exit = 1 << 2
        }
        [SerializeField] private When _when;

        [SerializeField] private bool _destroyParent;
        [SerializeField] private float _delay;

        [SerializeField] private UnityEvent<GameObject> _event;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_when.HasFlag(When.Enter))
            {
                Extensions.Miscellaneous.Helper.Delay(_delay, DelayAction, animator);
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_when.HasFlag(When.Update))
            {
                Extensions.Miscellaneous.Helper.Delay(_delay, DelayAction, animator);
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_when.HasFlag(When.Exit))
            {
                Extensions.Miscellaneous.Helper.Delay(_delay, DelayAction, animator);
            }
        }

        private void DelayAction(Animator animator)
        {
            if (_destroyParent)
                _event.Invoke(animator.transform.parent.gameObject);
            else
                _event.Invoke(animator.gameObject);
        }
    }
}
