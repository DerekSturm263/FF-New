using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class PopupController : SpawnableController<Popup>
    {
        [SerializeField] private GameObject _buttonTemplate;

        protected override void SetUp(Popup popup)
        {
            TMPro.TMP_Text[] texts = _templateInstance.transform.GetComponentsInChildren<TMPro.TMP_Text>();

            texts[0].SetText(popup.Title);
            texts[1].SetText(popup.Description);

            Transform buttonParent = _templateInstance.GetComponentInChildren<LayoutGroup>().transform;

            foreach (KeyValuePair<string, UnityEvent<PopupContinueContext>> response in popup.Responses)
            {
                Button button = Instantiate(_buttonTemplate, buttonParent).GetComponent<Button>();

                button.GetComponentInChildren<TMPro.TMP_Text>().SetText(response.Key);
                button.onClick.AddListener(() => response.Value.Invoke(null));
            }
        }

        public void ContinueAction(PopupContinueContext ctx) => ctx.InvokePreviousAction();
    }
}
