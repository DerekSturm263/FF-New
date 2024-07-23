using UnityEngine;

public class AnimationEventContainer : StateMachineBehaviour
{
    public QuantumAnimationEventAsset Event;

#if UNITY_EDITOR

    [HideInInspector] public UnityEditor.Animations.AnimatorState State;

    private void Reset()
    {
        State = UnityEditor.Selection.activeObject as UnityEditor.Animations.AnimatorState;

        Event = CreateAsset<QuantumAnimationEventAsset>(State.name);
        Event.Settings.AnimID = State.nameHash;
        Event.Settings.Events = new();
    }

    public static T CreateAsset<T>(string name, Object parent = null, System.Action<T> setup = null) where T : AssetBase
    {
        T selected = CreateInstance<T>();
        setup?.Invoke(selected);

        if (parent)
        {
            selected.name = name;
            UnityEditor.AssetDatabase.AddObjectToAsset(selected, parent);
        }
        else
        {
            selected.name = name;
            string assetPath = string.Format($"{QuantumEditorSettings.Instance.DefaultAssetSearchPath}/AnimationEvents/{name}.asset");
            assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);

            UnityEditor.AssetDatabase.CreateAsset(selected, assetPath);
        }

        UnityEditor.AssetDatabase.SaveAssets();

        return selected;
    }

#endif
}
