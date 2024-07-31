using Extensions.Types;
using Quantum;
using UnityEngine;

public class HurtboxTracker : MonoBehaviour
{
    [SerializeField] private Dictionary<HurtboxType, Transform> _hurtboxes;
    public Transform GetHurtbox(HurtboxType type) => _hurtboxes[type];

    private void Awake()
    {
        SetupHurtboxes();
    }

    private void Reset()
    {
        SetupHurtboxes();
    }

    private void SetupHurtboxes()
    {
        _hurtboxes = new()
        {
            [HurtboxType.Belly] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001"),
            [HurtboxType.LeftUpperLeg] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L"),
            [HurtboxType.LeftLowerLeg] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L/DEF-shin.L"),
            [HurtboxType.LeftFoot] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L/DEF-shin.L/DEF-foot.L"),
            [HurtboxType.LeftUpperArm] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L"),
            [HurtboxType.LeftForeArm] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L/DEF-forearm.L"),
            [HurtboxType.LeftHand] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L/DEF-forearm.L/DEF-hand.L"),
            [HurtboxType.RightUpperLeg] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R"),
            [HurtboxType.RightLowerLeg] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R/DEF-shin.R"),
            [HurtboxType.RightFoot] = transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R/DEF-shin.R/DEF-foot.R"),
            [HurtboxType.RightUpperArm] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R"),
            [HurtboxType.RightForeArm] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R/DEF-forearm.R"),
            [HurtboxType.RightHand] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R/DEF-forearm.R/DEF-hand.R"),
            [HurtboxType.NeckChest] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004"),
            [HurtboxType.Head] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004/DEF-neck/DEF-head"),
            [HurtboxType.MainWeapon] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R/DEF-forearm.R/DEF-hand.R/Main"),
            [HurtboxType.AltWeapon] = transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004/Alt")
        };
    }
}
