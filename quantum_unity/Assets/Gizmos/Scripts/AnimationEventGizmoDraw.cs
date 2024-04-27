using UnityEngine;

public class AnimationEventGizmoDraw : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (AnimationEventContainerWindow.Instance)
            AnimationEventContainerWindow.Instance.DrawGizmos();
    }
}
