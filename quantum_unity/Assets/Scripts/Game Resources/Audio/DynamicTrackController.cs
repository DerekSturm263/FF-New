using Extensions.Components.Miscellaneous;
using GameResources.Audio;
using Quantum;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class DynamicTrackController : Controller<DynamicTrackController>
{
    private GameObject _obj;
    public GameObject Obj => _obj = _obj ? _obj : CreateObj();
    private GameObject CreateObj()
    {
        return new GameObject("Stage Music Player");
    }

    private TrackGraph _trackGraph;
    private bool _isTransitioning;
    private float _transitionTime;
    private TrackSection _from;

    private Extensions.Types.Dictionary<TrackSection, List<AudioSource>> _trackSources;

    private List<DispatcherSubscription> _events = new();

    [SerializeField] private float _endDelay = 0.5f + (17.0f / 60);

    public override void Initialize()
    {
        base.Initialize();

        _isTransitioning = false;
        _transitionTime = 0;
        _trackSources = new();

        if (_trackGraph)
        {
            _trackGraph.InitializeSectionsDictionary();

            _trackGraph.SetCurrentSection(null);
            _trackGraph.SetCurrentTransition(null);

            _obj = CreateObj();

            foreach (TrackSection trackSection in _trackGraph.Sections)
            {
                GameObject section = new(trackSection.name);
                section.transform.parent = _obj.transform;
                _trackSources.Add(trackSection, new());

                foreach (AudioClip clip in trackSection.Clips)
                {
                    AudioSource source = section.AddComponent<AudioSource>();

                    source.clip = clip;
                    source.volume = 0;
                    source.loop = true;

                    _trackSources[trackSection].Add(source);
                }

                section.SetActive(false);
            }
        }

        _events.Add(QuantumEvent.Subscribe<EventOnPlayerLowHealth>(listener: this, handler: TensionTransition));
        _events.Add(QuantumEvent.Subscribe<EventOnOneMinuteLeft>(listener: this, handler: LastMinuteTransition));
    }

    private void TensionTransition(EventOnPlayerLowHealth e)
    {
        Extensions.Miscellaneous.Helper.Delay(_endDelay, () => TrackEventController.Instance.ExecuteOnNextBeat(() =>
        {
            ForceTransitionSmooth("Tension");
        }));
    }

    private void LastMinuteTransition(EventOnOneMinuteLeft e)
    {
        Extensions.Miscellaneous.Helper.Delay(_endDelay, () => TrackEventController.Instance.ExecuteOnNextBeat(() =>
        {
            TrackEventController.Instance.ResetFrame();
            ForceTransitionSmooth("Last Minute");
        }));
    }

    public override void UpdateDt(float dt)
    {
        if (!_trackGraph)
            return;
        
        List<float> weights = _trackGraph.CurrentSection.CalculateWeights(default);

        for (int i = 0; i < _trackSources[_trackGraph.CurrentSection].Count; ++i)
        {
            _trackSources[_trackGraph.CurrentSection][i].volume = weights[i];
        }

        if (_isTransitioning)
        {
            _transitionTime += dt;

            if (_transitionTime >= _trackGraph.CurrentTransition.Length)
                EndTransition(_from, _trackGraph.CurrentTransition);
            else
                UpdateTransition(_from, _trackGraph.CurrentTransition, _transitionTime);
        }
        else
        {
            if (_trackGraph.CurrentSection.ExitTime > 0 && _trackSources[_trackGraph.CurrentSection][0].time >= _trackGraph.CurrentSection.ExitTime)
                BeginTransition(_trackGraph.CurrentSection, _trackGraph.CurrentSection.Transitions[0]);
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();

        foreach (DispatcherSubscription quantumEvent in _events)
        {
            QuantumEvent.Unsubscribe(quantumEvent);
        }
    }

    public void Play(TrackGraph trackGraph)
    {
        _trackGraph = trackGraph;
    }

    private void BeginTransition(TrackSection from, TrackTransition transition)
    {
        _transitionTime = 0;
        _trackSources[_trackGraph.GetFromName(transition.To)][0].gameObject.SetActive(true);
        _trackGraph.SetCurrentTransition(transition);

        _from = _trackGraph.CurrentSection;
        _trackGraph.SetCurrentSection(_trackGraph.GetFromName(transition.To));

        if (transition.Clip != null)
            AudioSource.PlayClipAtPoint(transition.Clip, Vector3.zero);

        foreach (var source in _trackSources[from])
        {
            source.volume = 1;
        }
        foreach (var source in _trackSources[_trackGraph.GetFromName(transition.To)])
        {
            source.volume = 0;
        }

        _isTransitioning = true;
    }

    private void UpdateTransition(TrackSection from, TrackTransition transition, float t)
    {
        foreach (var source in _trackSources[from])
        {
            source.volume = 1 - t * (1 / transition.Length);
        }

        foreach (var source in _trackSources[_trackGraph.GetFromName(transition.To)])
        {
            source.volume = t * (1 / transition.Length);
        }
    }

    private void EndTransition(TrackSection from, TrackTransition transition)
    {
        foreach (var source in _trackSources[_trackGraph.GetFromName(transition.To)])
        {
            source.volume = 1;
        }

        if (from.KeepActive)
        {
            foreach (AudioSource audioSource in _trackSources[from])
            {
                audioSource.volume = 0;
            }
        }
        else
        {
            _trackSources[from][0].gameObject.SetActive(false);
        }
        
        _isTransitioning = false;
    }

    public void ForceTransition(string name)
    {
        if (_trackGraph.CurrentSection is not null && _trackSources.TryGetValue(_trackGraph.CurrentSection, out List<AudioSource> audioSources))
        {
            audioSources[0].gameObject.SetActive(false);
        }

        _trackGraph.SetCurrentSection(_trackGraph.GetFromName(name));
        _trackSources[_trackGraph.CurrentSection][0].gameObject.SetActive(true);

        TrackEventController.Instance.ResetFrame();
    }

    public void ForceTransitionSmooth(string name)
    {
        BeginTransition(_trackGraph.CurrentSection, _trackGraph.CurrentSection.Transitions.Find(item => item.To == name));
    }

    public void GetBPM(Extensions.Types.ReturningUnityEvent<int>.Resolver resolver)
    {
        resolver.SetReturnValue(_trackGraph.CurrentSection.BPM);
    }
}
