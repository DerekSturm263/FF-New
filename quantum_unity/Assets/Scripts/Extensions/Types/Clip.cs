using Extensions.Types;
using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/ClientAsset/Clip")]
public class Clip : ScriptableObject
{
    public RandomList<AudioClip> Variants;
    public AudioMixerGroup Mixer;
    public float Volume;
}
