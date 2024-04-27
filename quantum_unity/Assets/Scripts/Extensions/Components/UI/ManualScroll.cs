using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Manual Scroll", 79)]
    [DisallowMultipleComponent]
    public class ManualScroll : UIBehaviour
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private float _speed;

        [System.Obsolete("ForceStateNormalizedTime is deprecated but I couldn't find any replacement that does exactly what I want.")]
        public void ScrollBy(Vector2 amount)
        {
            if (amount == Vector2.zero)
                return;

            amount *= _speed * Time.deltaTime;

            _anim.ForceStateNormalizedTime(_anim.GetCurrentAnimatorStateInfo(0).normalizedTime + amount.y);
        }
    }
}
