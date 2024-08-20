using Extensions.Miscellaneous;
using UnityEditor;
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
    [SerializeField] private int _index;

    private CustomQuantumAnimator _customAnimator;
    private Animator _animator;

    private void Awake()
    {
        _customAnimator = GetComponentInParent<CustomQuantumAnimator>();
        _animator = _customAnimator.AnimData[_index].animator;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        var leftFoot = GetFootIK(AvatarIKGoal.LeftFoot, Color.red);
        var rightFoot = GetFootIK(AvatarIKGoal.RightFoot, Color.blue);

        if (leftFoot.HasValue)
        {
            var newFoot = _customAnimator.Direction < 0 ? (leftFoot.Value.Item1, new(leftFoot.Value.Item2.x, rightFoot.HasValue ? rightFoot.Value.Item2.y : leftFoot.Value.Item2.y)) : leftFoot;

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, newFoot.Value.Item1);
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, newFoot.Value.Item2);
        }
        if (rightFoot.HasValue)
        {
            var newFoot = _customAnimator.Direction < 0 ? (rightFoot.Value.Item1, new(rightFoot.Value.Item2.x, leftFoot.HasValue ? leftFoot.Value.Item2.y : rightFoot.Value.Item2.y)) : rightFoot;

            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, newFoot.Value.Item1);
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, newFoot.Value.Item2);
        }

        SetHeadIK(_animator);
    }

    public (float, Vector3)? GetFootIK(AvatarIKGoal goal, Color color)
    {
        float weight = _footWeight * _customAnimator.GetFloat(goal == AvatarIKGoal.LeftFoot ? 0 : 1);
        Vector3 footPos = goal == AvatarIKGoal.LeftFoot ? _customAnimator.LFoot.position : _customAnimator.RFoot.position;

        Debug.DrawLine(footPos, footPos - new Vector3(0, _footCastLength, 0), color);
        if (Physics.Linecast(footPos, footPos - new Vector3(0, _footCastLength, 0), out RaycastHit hit, _ground))
        {
            Vector3 newPos = hit.point + new Vector3(0, _footOffset, 0);
            Debug.DrawLine(newPos - new Vector3(0.1f, 0, 0.1f), newPos + new Vector3(0.1f, 0, 0.1f), color);

            return (weight, newPos);
        }

        return null;
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
