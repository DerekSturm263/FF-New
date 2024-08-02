using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Popup
{
    [CreateAssetMenu(fileName = "New Popup", menuName = "Fusion Fighters/UI/Popup")]
    public class Popup : ScriptableObject
    {
        [SerializeField] private string _title;
        public string Title => _title;

        [SerializeField][TextArea] private string _description;
        public string Description => _description;
        
        [SerializeField] private Extensions.Types.Dictionary<string, UnityEvent<PopupContinueContext>> _responses;
        public Extensions.Types.Dictionary<string, UnityEvent<PopupContinueContext>> Responses => _responses;

        [SerializeField] private Extensions.Types.Tuple<string, UnityEvent<string>> _inputResponse;
        public Extensions.Types.Tuple<string, UnityEvent<string>> InputResponse => _inputResponse;
    }
}
