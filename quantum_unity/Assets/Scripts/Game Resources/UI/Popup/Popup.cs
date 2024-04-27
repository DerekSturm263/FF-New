using UnityEngine;
using UnityEngine.Events;

namespace GameResources.UI.Popup
{
    [CreateAssetMenu(fileName = "New Popup", menuName = "Fusion Fighters/UI/Popup")]
    public class Popup : ScriptableObject
    {
        [SerializeField] private string _title;
        public string Title => _title;

        [SerializeField][Multiline] private string _description;
        public string Description => _description;
        
        [SerializeField] private Extensions.Types.Dictionary<string, UnityEvent<PopupContinueContext>> _responses;
        public Extensions.Types.Dictionary<string, UnityEvent<PopupContinueContext>> Responses => _responses;
        public void InvokeResponse(string index, PopupContinueContext ctx) => _responses[index].Invoke(ctx);
    }
}
