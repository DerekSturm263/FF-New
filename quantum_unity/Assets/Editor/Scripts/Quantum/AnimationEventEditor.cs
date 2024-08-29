using Quantum;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuantumAnimationEventAsset))]
public class AnimationEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QuantumAnimationEventAsset animationEvent = target as QuantumAnimationEventAsset;

        if (GUILayout.Button("Bake"))
        {
            foreach (AssetRefFrameEvent eventItem in animationEvent.Settings.Events)
            {
                var eventAsset = UnityDB.FindAsset<SpawnHitboxEventAsset>(eventItem.Id);
                if (eventAsset)
                {
                    eventAsset.InitForBaking();

                    for (int i = 0; i <= eventAsset.Settings.Length; ++i)
                    {
                        animationEvent.Clip.SampleAnimation(AnimationEventContainerWindow.Instance.PreviewPlayer, (i + eventAsset.Settings.StartingFrame) / animationEvent.Clip.frameRate);
                        eventAsset.Bake(i, AnimationEventContainerWindow.Instance.PreviewPlayer, AnimationEventContainerWindow.Instance.PreviewWeapon);
                    }
                }
            }
        }
    }
}
