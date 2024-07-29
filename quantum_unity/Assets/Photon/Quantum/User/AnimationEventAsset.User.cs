using UnityEngine;

public abstract unsafe partial class FrameEventAsset
{
    [TextArea][Tooltip("A custom description for identifying events")] public string Description;
    [Tooltip("A custom color for identifying events")] public Color Color = Color.white;
}
