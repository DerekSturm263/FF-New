using UnityEngine;

public class SetIK : StateMachineBehaviour
{
    [SerializeField] private LayerMask _ground;

    [SerializeField] private float _footWeight;
    [SerializeField] private float _footCastLength;
    [SerializeField] private float _footOffset;

    [SerializeField] private float _headWeight;
    [SerializeField] private float _headOffset;

    [SerializeField] private float _lerpSpeed;

    private CustomQuantumAnimator _animator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator.transform.parent.GetComponent<CustomQuantumAnimator>();
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawLine(_animator.LFoot.position, _animator.LFoot.position - new Vector3(0, _footCastLength, 0));
        if (Physics.Linecast(_animator.LFoot.position, _animator.LFoot.position - new Vector3(0, _footCastLength, 0), out RaycastHit hit1, _ground))
        {
            Vector3 oldPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            Vector3 newPos = hit1.point + new Vector3(0, _footOffset, 0);

            Debug.DrawLine(newPos - new Vector3(0.1f, 0, 0.1f), newPos + new Vector3(0.1f, 0, 0.1f));

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _footWeight * _animator.GetFloat(0));
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, Vector3.Lerp(oldPos, newPos, Time.deltaTime * _lerpSpeed));
        }

        Debug.DrawLine(_animator.RFoot.position, _animator.RFoot.position - new Vector3(0, _footCastLength, 0));
        if (Physics.Linecast(_animator.RFoot.position, _animator.RFoot.position - new Vector3(0, _footCastLength, 0), out RaycastHit hit2, _ground))
        {
            Vector3 oldPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
            Vector3 newPos = hit2.point + new Vector3(0, _footOffset, 0);

            Debug.DrawLine(newPos - new Vector3(0.1f, 0, 0.1f), newPos + new Vector3(0.1f, 0, 0.1f));

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _footWeight * _animator.GetFloat(1));
            animator.SetIKPosition(AvatarIKGoal.RightFoot, Vector3.Lerp(oldPos, newPos, Time.deltaTime * _lerpSpeed));
        }

        if (_animator.Target)
        {
            Vector3 newPos = _animator.Target.Head.transform.position - _animator.Head.transform.position + new Vector3(0, _headOffset, 0);

            Debug.DrawLine(_animator.Head.transform.position, newPos);

            animator.SetLookAtWeight(_headWeight * _animator.GetFloat(2));
            animator.SetLookAtPosition(newPos);
        }
    }
}
