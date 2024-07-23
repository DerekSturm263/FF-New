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
        SetFootIK(animator, AvatarIKGoal.LeftFoot, _animator.Direction < 0);
        SetFootIK(animator, AvatarIKGoal.RightFoot, _animator.Direction < 0);
        SetHeadIK(animator);
    }

    public void SetFootIK(Animator animator, AvatarIKGoal goal, bool isReversed)
    {
        Vector3 footPos = goal == AvatarIKGoal.LeftFoot ^ isReversed ? _animator.LFoot.position : _animator.RFoot.position;

        Debug.DrawLine(footPos, footPos - new Vector3(0, _footCastLength, 0));
        if (Physics.Linecast(footPos, footPos - new Vector3(0, _footCastLength, 0), out RaycastHit hit, _ground))
        {
            Vector3 oldPos = animator.GetIKPosition(goal);
            Vector3 newPos = hit.point + new Vector3(0, _footOffset, 0);

            Debug.DrawLine(newPos - new Vector3(0.1f, 0, 0.1f), newPos + new Vector3(0.1f, 0, 0.1f));

            animator.SetIKPositionWeight(goal, _footWeight * _animator.GetFloat(goal == AvatarIKGoal.LeftFoot ? 0 : 1));
            animator.SetIKPosition(goal, Vector3.Lerp(oldPos, newPos, Time.deltaTime * _lerpSpeed));
        }
    }

    public void SetHeadIK(Animator animator)
    {
        if (!_animator.Target)
            return;

        Vector3 newPos = _animator.Target.Head.transform.position - _animator.Head.transform.position + new Vector3(0, _headOffset, 0);

        Debug.DrawLine(_animator.Head.transform.position, newPos);

        animator.SetLookAtWeight(_headWeight * _animator.GetFloat(2));
        animator.SetLookAtPosition(newPos);
    }
}
