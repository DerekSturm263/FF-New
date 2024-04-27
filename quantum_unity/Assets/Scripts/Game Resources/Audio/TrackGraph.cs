using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/ClientAsset/Track")]
public class TrackGraph : ScriptableObject
{
    public List<TrackSection> Sections;
    public TrackSection GetFromName(string name) => Sections.First(item => item.name == name);

    private TrackSection _currentSection;
    public TrackSection CurrentSection => _currentSection;
    public void SetCurrentSection(TrackSection current) => _currentSection = current;

    private TrackTransition _currentTransition;
    public TrackTransition CurrentTransition => _currentTransition;
    public void SetCurrentTransition(TrackTransition currentTransition) => _currentTransition = currentTransition;

    public void SetWeightOne(EntityViewUpdater entityViewUpdater, Extensions.Types.ReturningUnityEvent<EntityViewUpdater, List<float>>.Resolver resolver) => resolver.SetReturnValue(new List<float>() { 1 });
    public void SetWeightZero(EntityViewUpdater entityViewUpdater, Extensions.Types.ReturningUnityEvent<EntityViewUpdater, List<float>>.Resolver resolver) => resolver.SetReturnValue(new List<float>() { 0 });
    public void SetWeightsOneZero(EntityViewUpdater entityViewUpdater, Extensions.Types.ReturningUnityEvent<EntityViewUpdater, List<float>>.Resolver resolver) => resolver.SetReturnValue(new List<float>() { 1, 0 });
}
