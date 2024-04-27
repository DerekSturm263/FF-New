using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class PopupController : Extensions.Components.Miscellaneous.Controller<PopupController>
    {
        [SerializeField] private GameObject _popupTemplate;
        [SerializeField] private GameObject _buttonTemplate;

        private GameObject _popupInstance;
        private GameObject _oldSelected;

        public void DisplayPopup(Popup popup)
        {
            GameObject parent = GameObject.FindGameObjectWithTag("Popup Canvas");

            GameObject newPopup = Instantiate(_popupTemplate, parent.transform);
            
            TMPro.TMP_Text[] texts = newPopup.transform.GetComponentsInChildren<TMPro.TMP_Text>();
            texts[0].SetText(popup.Title);
            texts[1].SetText(popup.Description);

            RememberOldSelected();
            Transform buttonParent = newPopup.GetComponentInChildren<LayoutGroup>().transform;

            foreach (KeyValuePair<string, UnityEvent<PopupContinueContext>> response in popup.Responses)
            {
                Button button = Instantiate(_buttonTemplate, buttonParent).GetComponent<Button>();

                button.GetComponentInChildren<TMPro.TMP_Text>().SetText(response.Key);
                button.onClick.AddListener(() => response.Value.Invoke(null));

                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }

            _popupInstance = newPopup;
        }

        public void ClosePopup()
        {
            _popupInstance.GetComponent<PopupInstance>().InvokeOnAnimationEndStart();
            _popupInstance.GetComponent<Animator>().SetTrigger("Exit");

            SetOldSelected();
        }

        public void RememberOldSelected()
        {
            if (EventSystem.current)
                _oldSelected = EventSystem.current.currentSelectedGameObject;
        }

        public void SetOldSelected()
        {
            if (EventSystem.current)
                EventSystem.current.SetSelectedGameObject(_oldSelected);
        }

        public void ContinueAction(PopupContinueContext ctx) => ctx.InvokePreviousAction();
    }
}
