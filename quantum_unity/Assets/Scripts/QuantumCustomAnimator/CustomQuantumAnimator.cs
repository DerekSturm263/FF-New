using Quantum;
using Quantum.Custom.Animator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

using UE = UnityEngine;

public struct AnimationData
{
    public UE.Animator animator;
    public PlayableGraph graph;
    public AnimationMixerPlayable mixerPlayable;
    public readonly Dictionary<string, KeyValuePair<int, AnimationClipPlayable>> clips;

    public AnimationData(Animator animator, PlayableGraph graph, AnimationMixerPlayable mixerPlayable)
    {
        this.animator = animator;
        this.graph = graph;
        this.mixerPlayable = mixerPlayable;
        this.clips = new();
    }

    public void Setup(int index)
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        mixerPlayable = AnimationMixerPlayable.Create(graph);
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, $"Animation {index}", animator);
        output.SetSourcePlayable(mixerPlayable);

        graph.Play();
    }
}

public unsafe class CustomQuantumAnimator : MonoBehaviour
{
    private AnimationData[] _animData;
    public AnimationData[] AnimData => _animData;

    private readonly List<int> _activeInputs = new(64);
    private static readonly List<AnimatorRuntimeBlendData> _blendData = new(64);
    private static readonly List<AnimatorMotion> _motionData = new(32);

    private CustomQuantumAnimator _target;
    public CustomQuantumAnimator Target => _target;
    public void SetTarget(CustomQuantumAnimator target) => _target = target;

    [SerializeField] private Transform _lFoot;
    public Transform LFoot => _lFoot;

    [SerializeField] private Transform _rFoot;
    public Transform RFoot => _rFoot;

    [SerializeField] private Transform _head;
    public Transform Head => _head;

    private float[] _curveValues = new float[3];

    [SerializeField] private float _speed;

    private int _direction;
    public int Direction => _direction;

    public float GetFloat(int index)
    {
        return _curveValues[index];
    }

    private void Awake()
    {
        _animData = GetComponentsInChildren<UE.Animator>().Select(item => new AnimationData(item, new(), new())).ToArray();

        QuantumEvent.Subscribe<EventOnPlayerChangeDirection>(listener: this, handler: e => _direction = e.Direction);
    }

    private void OnEnable()
    {
        for (int i = 0; i < _animData.Length; ++i)
        {
            _animData[i].Setup(i);
        }
    }

    private void OnDisable()
    {
        _activeInputs.Clear();

        for (int i = 0; i < _animData.Length; ++i)
        {
            _animData[i].clips.Clear();
            _animData[i].graph.Destroy();
        }
    }

    public void Animate(Quantum.Frame frame, Quantum.CustomAnimator* a)
    {
        var asset = UnityDB.FindAsset<CustomAnimatorGraphAsset>(a->animatorGraph.Id);

        if (asset)
        {
            // load clips
            LoadClips(asset.clips);

            // calculate blend data
            asset.Settings.GenerateBlendList(frame, a, _blendData);

            // update animation state
            SetAnimationData(asset.Settings, _blendData);

            // clear old blend data
            _blendData.Clear();

            // get the custom animator curves
            _curveValues[0] = Mathf.Lerp(_curveValues[0], CustomAnimator.GetFP(frame, a, 0).AsFloat, frame.DeltaTime.AsFloat * _speed);
            _curveValues[1] = Mathf.Lerp(_curveValues[1], CustomAnimator.GetFP(frame, a, 1).AsFloat, frame.DeltaTime.AsFloat * _speed);
            _curveValues[2] = Mathf.Lerp(_curveValues[2], CustomAnimator.GetFP(frame, a, 2).AsFloat, frame.DeltaTime.AsFloat * _speed);
        }
    }

    private void LoadClips(List<AnimationClip> clipList)
    {
        for (int i = 0; i < _animData.Length; ++i)
        {
            if (_animData[i].clips.Count == 0)
            {
                for (int j = 0; j < clipList.Count; ++j)
                {
                    if (!_animData[i].clips.ContainsKey(clipList[j].name))
                    {
                        AnimationClipPlayable clip = AnimationClipPlayable.Create(_animData[i].graph, clipList[j]);
                        
                        clip.SetApplyFootIK(true);
                        clip.SetApplyPlayableIK(true);

                        _animData[i].clips.Add
                        (
                            clipList[j].name,
                            new KeyValuePair<int, AnimationClipPlayable>(_animData[i].mixerPlayable.AddInput(clip, 0), clip)
                        );

                        clip.Pause();
                    }
                }
            }
        }
    }

    private void SetAnimationData(CustomAnimatorGraph graph, List<AnimatorRuntimeBlendData> blendData)
    {
        foreach (int input in _activeInputs)
        {
            for (int i = 0; i < _animData.Length; ++i)
            {
                if (!_animData[i].animator.enabled)
                    continue;

                _animData[i].mixerPlayable.SetInputWeight(input, 0);
            }
        }

        _activeInputs.Clear();

        foreach (AnimatorRuntimeBlendData b in blendData)
        {
            AnimatorState state = graph.GetState(b.stateId);

            if (state.GetMotion(b.animationIndex, _motionData) is AnimatorClip motion && !string.IsNullOrEmpty(motion.clipName))
            {
                for (int i = 0; i < _animData.Length; ++i)
                {
                    if (!_animData[i].animator.enabled)
                        continue;

                    if (_animData[i].clips.TryGetValue(motion.clipName, out KeyValuePair<int, AnimationClipPlayable> clip))
                    {
                        _activeInputs.Add(clip.Key);

                        _animData[i].mixerPlayable.SetInputWeight(clip.Key, b.weight.AsFloat);

                        double normalTime = b.normalTime.AsDouble;
                        float clipLength = clip.Value.GetAnimationClip().length;
                        double expectedTime = normalTime * clipLength;

                        clip.Value.SetTime(expectedTime);
                    }
                    else
                    {
                        Log.Error("SetAnimationData failed to find clip: " + motion.clipName + " in graph: " + graph.Guid.ToString());
                    }
                }
            }

            _motionData.Clear();
        }
    }
}
