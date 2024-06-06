using Extensions.Types;
using UnityEngine.Audio;
using UnityEngine;
using Quantum;

[CreateAssetMenu(menuName = "Quantum/ClientAsset/Clip")]
public class Clip : ScriptableObject
{
    [SerializeField] private Dictionary<AssetRefVoice, RandomList<AudioClip>> Variants;
    
    public AudioClip GetClip(AssetRefVoice voice) => Variants[voice].Random;

    public AudioMixerGroup Mixer;
    public float Volume;
}
