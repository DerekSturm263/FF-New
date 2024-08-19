using UnityEngine;

public class SetIK : MonoBehaviour
{
    [SerializeField] private LayerMask _ground;

    [SerializeField] private float _footWeight;
    [SerializeField] private float _footCastLength;
    [SerializeField] private float _footOffset;

    [SerializeField] private float _headWeight;
    [SerializeField] private float _headOffset;

    [SerializeField] private float _lerpSpeed;

    private CustomQuantumAnimator _customAnimator;
    private Animator _animator;

    private void Awake()
    {
        _customAnimator = GetComponentInParent<CustomQuantumAnimator>();
        _animator = _customAnimator.AnimData[0].animator;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        SetFootIK(_animator, AvatarIKGoal.LeftFoot);
        SetFootIK(_animator, AvatarIKGoal.RightFoot);
        SetHeadIK(_animator);
    }

    public void SetFootIK(Animator animator, AvatarIKGoal goal)
    {
        Vector3 footPos = goal == AvatarIKGoal.LeftFoot ? _customAnimator.LFoot.position : _customAnimator.RFoot.position;

        Debug.DrawLine(footPos, footPos - new Vector3(0, _footCastLength, 0));
        if (Physics.Linecast(footPos, footPos - new Vector3(0, _footCastLength, 0), out RaycastHit hit, _ground))
        {
            Vector3 oldPos = animator.GetIKPosition(goal);
            Vector3 newPos = hit.point + new Vector3(0, _footOffset, 0);

            Debug.DrawLine(newPos - new Vector3(0.1f, 0, 0.1f), newPos + new Vector3(0.1f, 0, 0.1f));

            animator.SetIKPositionWeight(goal, _footWeight * _customAnimator.GetFloat(goal == AvatarIKGoal.LeftFoot ? 0 : 1));
            animator.SetIKPosition(goal, Vector3.Lerp(oldPos, newPos, Time.deltaTime * _lerpSpeed));
        }
    }

    public void SetHeadIK(Animator animator)
    {
        if (!_customAnimator.Target)
            return;

        Vector3 newPos = _customAnimator.Target.Head.transform.position - _customAnimator.Head.transform.position + new Vector3(0, _headOffset, 0);

        Debug.DrawLine(_customAnimator.Head.transform.position, newPos);

        animator.SetLookAtWeight(_headWeight * _customAnimator.GetFloat(2));
        animator.SetLookAtPosition(newPos);
    }
}
