using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Dialogue
{
    [System.Serializable]
    public class Dialogue
    {
        [SerializeField] private string _speaker;
        public string Speaker => _speaker;

        [SerializeField] private List<DialoguePiece> _dialogues;
        public List<DialoguePiece> Dialogues => _dialogues;

        [SerializeField] private UnityEvent _onDialogueEnd;
        public void InvokeOnDialogueEnd() => _onDialogueEnd?.Invoke();
    }
}
