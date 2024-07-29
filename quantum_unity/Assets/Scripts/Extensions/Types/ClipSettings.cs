using Extensions.Types;
using Quantum;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct ClipSettings
{
    [SerializeField] private Dictionary<AssetRefVoice, RandomList<AudioClip>> Variants;
    public AudioClip GetClip(AssetRefVoice voice) => Variants[voice].Random;

    public AudioMixerGroup Mixer;
    public float Volume;
}
