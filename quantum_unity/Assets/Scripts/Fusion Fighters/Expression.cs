using Extensions.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expression", menuName = "Fusion Fighters/Expression")]
public class Expression : ScriptableObject
{
    [SerializeField] private Nullable<float> _openEyesAmount;
    public Nullable<float> OpenEyesAmount => _openEyesAmount;

    [SerializeField] private Nullable<float> _openMouthAmount;
    public Nullable<float> OpenMouthAmount => _openMouthAmount;

    [SerializeField] private Nullable<float> _smileAmount;
    public Nullable<float> SmileAmount => _smileAmount;

    [SerializeField] private Nullable<float> _cuteMouthAmount;
    public Nullable<float> CuteMouthAmount => _cuteMouthAmount;

    [SerializeField] private Nullable<float> _happyMouthAmount;
    public Nullable<float> HappyMouthAmount => _happyMouthAmount;

    [SerializeField] private Nullable<float> _tongueStickAmount;
    public Nullable<float> TongueStickAmount => _tongueStickAmount;

    [SerializeField] private Nullable<float> _angryEyesAmount;
    public Nullable<float> AngryEyesAmount => _angryEyesAmount;

    [SerializeField] private Nullable<float> _angryMouthAmount;
    public Nullable<float> AngryMouthAmount => _angryMouthAmount;

    [SerializeField] private Nullable<float> _sadEyesAmount;
    public Nullable<float> SadEyesAmount => _sadEyesAmount;

    [SerializeField] private Nullable<float> _sadMouthAmount;
    public Nullable<float> SadMouthAmount => _sadMouthAmount;

    [SerializeField] private Nullable<float> _wideEyesAmount;
    public Nullable<float> WideEyesAmount => _wideEyesAmount;
}
