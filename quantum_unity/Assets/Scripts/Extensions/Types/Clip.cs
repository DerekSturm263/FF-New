using Extensions.Types;
using UnityEngine.Audio;
using UnityEngine;
using Quantum;

[CreateAssetMenu(menuName = "Quantum/ClientAsset/Clip")]
public class Clip : ScriptableObject
{
    [SerializeField] private Dictionary<AssetRefFFAvatar, RandomList<AudioClip>> Variants;
    
    public AudioClip GetClip(AssetRefFFAvatar avatar) => Variants[avatar].Random;

    public AudioMixerGroup Mixer;
    public float Volume;
}
