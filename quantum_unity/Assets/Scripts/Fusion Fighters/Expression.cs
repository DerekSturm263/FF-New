using Extensions.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expression", menuName = "Fusion Fighters/Expression")]
public class Expression : ScriptableObject
{
    [SerializeField] private Range<float> _length;
    public Range<float> Length => _length;

    [SerializeField] private Nullable<AnimationCurve> _openEyesAmount;
    public Nullable<AnimationCurve> OpenEyesAmount => _openEyesAmount;

    [SerializeField] private Nullable<AnimationCurve> _openMouthAmount;
    public Nullable<AnimationCurve> OpenMouthAmount => _openMouthAmount;

    [SerializeField] private Nullable<AnimationCurve> _smileAmount;
    public Nullable<AnimationCurve> SmileAmount => _smileAmount;

    [SerializeField] private Nullable<AnimationCurve> _cuteMouthAmount;
    public Nullable<AnimationCurve> CuteMouthAmount => _cuteMouthAmount;

    [SerializeField] private Nullable<AnimationCurve> _happyMouthAmount;
    public Nullable<AnimationCurve> HappyMouthAmount => _happyMouthAmount;

    [SerializeField] private Nullable<AnimationCurve> _tongueStickAmount;
    public Nullable<AnimationCurve> TongueStickAmount => _tongueStickAmount;

    [SerializeField] private Nullable<AnimationCurve> _angryEyesAmount;
    public Nullable<AnimationCurve> AngryEyesAmount => _angryEyesAmount;

    [SerializeField] private Nullable<AnimationCurve> _angryMouthAmount;
    public Nullable<AnimationCurve> AngryMouthAmount => _angryMouthAmount;

    [SerializeField] private Nullable<AnimationCurve> _sadEyesAmount;
    public Nullable<AnimationCurve> SadEyesAmount => _sadEyesAmount;

    [SerializeField] private Nullable<AnimationCurve> _sadMouthAmount;
    public Nullable<AnimationCurve> SadMouthAmount => _sadMouthAmount;

    [SerializeField] private Nullable<AnimationCurve> _wideEyesAmount;
    public Nullable<AnimationCurve> WideEyesAmount => _wideEyesAmount;
}
