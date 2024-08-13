using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.Audio;

namespace GameResources.Audio
{
    /// <summary>
    /// Exposes methods to easily play music and sound effects.
    /// </summary>
    public class AudioController : Extensions.Components.Miscellaneous.Controller<AudioController>
    {
        private AudioSource _musicSource;

        private AudioSource CreateMusicSource()
        {
            AudioSource audioSource = new GameObject("UI Music Player").AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = _uiMusicGroup;

            DontDestroyOnLoad(audioSource.gameObject);
            return audioSource;
        }

        private AudioSource _sfxSource;

        private AudioSource CreateSFXSource()
        {
            AudioSource audioSource = new GameObject("UI SFX Player").AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = _uiSFXGroup;

            DontDestroyOnLoad(audioSource.gameObject);
            return audioSource;
        }

        [SerializeField] [Tooltip("The AudioMixerGroup for playing any UI-related audio.")] private AudioMixerGroup _uiGroup;
        [SerializeField] [Tooltip("The AudioMixerGroup for playing UI-related music.")] private AudioMixerGroup _uiMusicGroup;
        [SerializeField] [Tooltip("The AudioMixerGroup for playing UI-related sound effects.")] private AudioMixerGroup _uiSFXGroup;
        
        public override void Initialize()
        {
            // Make sure to call the base class's initialize.
            base.Initialize();

            // Create the music and sound effect sources.
            if (!_musicSource)
                _musicSource = CreateMusicSource();
            
            if (!_sfxSource)
                _sfxSource = CreateSFXSource();
        }

        public override void Shutdown()
        {
            // Destroy the music Source.
            if (_musicSource)
                Destroy(_musicSource.gameObject);

            // Destroy the sound effect source.
            if (_sfxSource)
                Destroy(_sfxSource.gameObject);

            // Make sure to call the base class's shutdown.
            base.Shutdown();
        }

        /// <summary>
        /// Plays an AudioClip.
        /// </summary>
        /// <param name="clip">The clip to be played.</param>
        public void PlayAudioClip(AudioClip clip)
        {
            if (!_sfxSource)
                return;

            Debug.Log($"{clip.name} played");
            _sfxSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Stops an AudioClip.
        /// </summary>
        public void StopAudioClip() => _sfxSource.Stop();

        /// <summary>
        /// Plays an AudioClip in the Music Source.
        /// </summary>
        /// <param name="clip">The clip to be played.</param>
        public void PlayAudioClipAsTrack(AudioClip clip)
        {
            // Set the track.
            _musicSource.clip = clip;
            PlayTrack();
        }

        public unsafe void PlayVoiceLine(QuantumGame game, EntityView user, (EntityView itemObj, ItemAsset itemAsset, FPVector2 position) tuple)
        {
            AudioClip clip = tuple.itemAsset.SFX["Voice Line"].Settings.GetClip(game.Frames.Verified.Unsafe.GetPointer<PlayerStats>(user.EntityRef)->Build.Frame.Voice);

            if (clip)
                user.GetComponentInChildren<AudioSource>().PlayOneShot(clip, tuple.itemAsset.SFX["Voice Line"].Settings.Volume);
        }

        /// <summary>
        /// Sets the Track to be played by the AudioController. Does not play the Track!
        /// </summary>
        /// <param name="track">Track to be loaded.</param>
        public void SetTrack(TrackGraph track)
        {
            // Set the track.
            _musicSource.clip = track.GetUnityAsset().GetFromName("Normal").Clips[0];
        }
        
        /// <summary>
        /// Plays the Track currently loaded into the AudioController. A Track must already be set!
        /// </summary>
        public void PlayTrack()
        {
            // Play the currently set track.
            if (!_musicSource.isPlaying)
                _musicSource.Play();
        }
        
        /// <summary>
        /// Pauses the Track currently being played by the AudioController.
        /// </summary>
        public void PauseTrack() => _musicSource.Pause();

        /// <summary>
        /// Stops the Track currently being played by the AudioController.
        /// </summary>
        public void StopTrack() => _musicSource.Stop();

        /// <summary>
        /// Enables an AudioMixerSnapshot at 33% strength.
        /// </summary>
        /// <param name="snapshot">The snapshot to be enabled.</param>
        public void EnableSnapshot(AudioMixerSnapshot snapshot) => snapshot.TransitionTo(0.33333f);
    }
}
