using GameResources.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [SerializeField][Multiline] private string _text;
        public string Text => _text;

        [SerializeField] private float _speed;
        public float Speed => _speed;

        [SerializeField] private AudioClip[] _voiceLines;
        public void PlayVoiceLines(DialogueController caller)
        {
            float delay = 0;

            foreach (AudioClip voiceLine in _voiceLines)
            {
                caller.StartCoroutine(PlayClipOnDelay(delay, voiceLine));
                delay += voiceLine.length - 1.5f;
            }
        }

        private IEnumerator PlayClipOnDelay(float delay, AudioClip clip)
        {
            yield return new WaitForSeconds(delay);

            AudioController.Instance.StopAudioClip();
            AudioController.Instance.PlayAudioClip(clip);
        }

        [SerializeField] private UnityEvent _event;
        public void InvokeEvent() => _event?.Invoke();

        [SerializeField] private Extensions.Types.Dictionary<string, UnityEvent> _responses;
        public Extensions.Types.Dictionary<string, UnityEvent> Responses => _responses;
        public void InvokeResponse(string index) => _responses[index].Invoke();

        public DialoguePiece(string text, float speed, AudioClip[] voiceLines, UnityEvent unityEvent, Extensions.Types.Dictionary<string, UnityEvent> responses)
        {
            _text = text;
            _speed = speed;
            _voiceLines = voiceLines;
            _event = unityEvent;
            _responses = responses;
        }
    }
}
