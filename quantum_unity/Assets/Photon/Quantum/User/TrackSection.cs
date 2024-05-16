using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackSection
{
    public string name;
    public int BPM;
    public float ExitTime;
    public bool KeepActive;

    public List<AudioClip> Clips;

    [SerializeField] private List<float> _defaultWeights;
    public List<float> DefaultWeights => _defaultWeights;

    [SerializeField] private Extensions.Types.ReturningUnityEvent<EntityViewUpdater, List<float>> _calculateWeights;
    public List<float> CalculateWeights(EntityViewUpdater entityViewUpdater) => _calculateWeights.Invoke(entityViewUpdater);

    public List<TrackTransition> Transitions;
}

[System.Serializable]
public class TrackTransition
{
    public string To;
    public AudioClip Clip;
    public float Length;
}
