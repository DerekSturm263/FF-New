using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Fusion Fighters/UI/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private bool _hideSpeaker;
        public bool HideSpeaker => _hideSpeaker;

        [SerializeField] private string _speaker;
        public string Speaker => _speaker;

        [SerializeField] private List<DialoguePiece> _dialogues;
        public List<DialoguePiece> Dialogues => _dialogues;

        [SerializeField] private float _eventDelay;
        public float EventDelay => _eventDelay;

        [SerializeField] private UnityEvent _onDialogueEnd;
        public void InvokeOnDialogueEnd() => _onDialogueEnd?.Invoke();
    }
}
