using Extensions.Types;
using UnityEngine;

public class HurtboxTracker : MonoBehaviour
{
    [SerializeField] private Dictionary<Quantum.HurtboxType, Transform> _hurtboxes;
    public Transform GetHurtbox(Quantum.HurtboxType type) => _hurtboxes[type];
}
