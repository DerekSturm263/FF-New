using System.Collections.Generic;
using UnityEngine;

public class ParentClothing : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;
    [SerializeField] private bool _isParent;

    private ParentClothing _parent;
    public void SetParent(ParentClothing parent) => _parent = parent;

    private void Awake()
    {
        if (!_isParent)
            SetupHurtboxes();
    }

    private void LateUpdate()
    {
        if (_isParent)
            return;

        for (int i = 0; i < _points.Count; ++i)
        {
            _points[i].position = _parent._points[i].position;
            _points[i].rotation = _parent._points[i].rotation;
        }
    }

    private void Reset()
    {
        SetupHurtboxes();
    }

    private void SetupHurtboxes()
    {
        _points = new()
        {
            transform.Find("Body_Rig/root/DEF-spine"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L/DEF-shin.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L/DEF-shin.L/DEF-foot.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.L/DEF-shin.L/DEF-foot.L/DEF-toe.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R/DEF-shin.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R/DEF-shin.R/DEF-foot.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-thigh.R/DEF-shin.R/DEF-foot.R/DEF-toe.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L/DEF-forearm.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.L/DEF-upper_arm.L/DEF-forearm.L/DEF-hand.L"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R/DEF-forearm.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-shoulder.R/DEF-upper_arm.R/DEF-forearm.R/DEF-hand.R"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004/DEF-neck"),
            transform.Find("Body_Rig/root/DEF-spine/DEF-spine.001/DEF-spine.002/DEF-spine.003/DEF-spine.004/DEF-neck/DEF-head")
        };
    }
}
