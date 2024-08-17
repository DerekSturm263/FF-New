using Extensions.Components.Miscellaneous;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class PopupController : SpawnableController<Popup>
    {
        [SerializeField] private GameObject _buttonTemplate;
        [SerializeField] private GameObject _inputFieldTemplate;

        private PopupContinueContext _continueContext;

        protected override bool TakeAwayFocus() => true;

        public void InsertEvent(InvokableGameObject ctx)
        {
            _continueContext = new(() => ctx.Invoke());
        }

        public void ClearEvent()
        {
            _continueContext = null;
        }

        protected override void SetUp(Popup popup)
        {
            TMPro.TMP_Text[] texts = _templateInstance.transform.GetComponentsInChildren<TMPro.TMP_Text>();
            var layoutGroups = _templateInstance.GetComponentsInChildren<LayoutGroup>();

            texts[0].SetText(popup.Title);
            texts[1].SetText(popup.Description);

            Transform buttonParent = layoutGroups[0].transform;
            foreach (KeyValuePair<string, UnityEvent<PopupContinueContext>> response in popup.Responses)
            {
                Button button = Instantiate(_buttonTemplate, buttonParent).GetComponent<Button>();

                button.GetComponentInChildren<TMPro.TMP_Text>().SetText(response.Key);
                button.onClick.AddListener(() => response.Value.Invoke(_continueContext));
            }

            Transform inputFieldParent = layoutGroups[1].transform;
            if (popup.InputResponse.Item2 is not null && popup.InputResponse.Item2.GetPersistentEventCount() > 0)
            {
                TMPro.TMP_InputField inputField = Instantiate(_inputFieldTemplate, inputFieldParent).GetComponentInChildren<TMPro.TMP_InputField>();

                inputField.SetTextWithoutNotify(popup.InputResponse.Item1);
                inputField.onEndEdit.AddListener(popup.InputResponse.Item2.Invoke);
            }
        }

        public void ContinueAction(PopupContinueContext ctx)
        {
            ctx?.InvokePreviousAction();
            ClearEvent();
        }
    }
}
