using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private float _speed;

    public void ScrubAnimation(float amount)
    {
        if (amount == 0)
            return;

        amount *= _speed * Time.deltaTime;
        float newAmount = _anim.GetCurrentAnimatorStateInfo(0).normalizedTime + amount;
        if (newAmount > 1)
            newAmount = 1;
        else if (newAmount < 0)
            newAmount = 0;

        _anim.ForceStateNormalizedTime(newAmount);
    }
}
