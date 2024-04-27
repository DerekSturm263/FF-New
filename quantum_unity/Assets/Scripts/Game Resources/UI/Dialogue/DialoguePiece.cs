using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Dialogue
{
    [System.Serializable]
    public struct DialoguePiece
    {
        [SerializeField][Multiline] private string _text;
        public readonly string Text => _text;

        [SerializeField] private float _speed;
        public readonly float Speed => _speed;

        [SerializeField] private UnityEvent _event;
        public readonly void InvokeEvent() => _event?.Invoke();

        [SerializeField] private Extensions.Types.Dictionary<string, UnityEvent> _responses;
        public readonly Extensions.Types.Dictionary<string, UnityEvent> Responses => _responses;
        public readonly void InvokeResponse(string index) => _responses[index].Invoke();

        public DialoguePiece(string text, float speed, UnityEvent unityEvent, Extensions.Types.Dictionary<string, UnityEvent> responses)
        {
            _text = text;
            _speed = speed;
            _event = unityEvent;
            _responses = responses;
        }
    }
}
