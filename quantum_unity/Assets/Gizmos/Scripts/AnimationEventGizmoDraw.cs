using Quantum;
using UnityEngine;

public class AnimationEventGizmoDraw : MonoBehaviour
{
    [SerializeField] private AssetRefHurtboxSetup _hurtboxSetup;

    private void OnDrawGizmos()
    {
        if (AnimationEventContainerWindow.Instance)
            AnimationEventContainerWindow.Instance.DrawGizmos(_hurtboxSetup);
    }
}
