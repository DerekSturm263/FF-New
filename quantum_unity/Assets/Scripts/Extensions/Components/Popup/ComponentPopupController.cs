using Extensions.Components.Input;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameResources.UI.Popup
{
    public class ComponentPopupController<TComponent, T> where TComponent : MonoBehaviour
    {
        private static GameObject _templateInstance;

        public static void Spawn(string title, GameObject template, Action<TComponent> action, TComponent templatePopulator)
        {
            EventSystemController.Instance.Enable();

            Transform parent = GameObject.FindWithTag("Popup Canvas").transform;
            _templateInstance = UnityEngine.Object.Instantiate(template, parent);

            SetUp(title, action, templatePopulator);
        }

        public static void Despawn()
        {
            _templateInstance.GetComponent<Animator>().SetTrigger("Exit");

            if (_templateInstance.TryGetComponent(out FadeOutEvents fadeOutEvents))
            {
                fadeOutEvents.InvokeOnClose();
                fadeOutEvents.DisableButtons();
            }

            EventSystemController.Instance.Disable();
        }

        public static void SetUp(string title, Action<TComponent> action, TComponent templatePopulator)
        {
            TComponent component = UnityEngine.Object.Instantiate(templatePopulator);
            component.gameObject.SetActive(true);

            component.transform.SetParent(_templateInstance.transform.GetChild(1));

            component.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            component.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            component.transform.localScale = Vector3.one;

            InputEvent inputEvent = component.gameObject.GetComponent<InputEvent>();
            inputEvent.OnAction.AddListener(Despawn);

            component.transform.parent.GetComponentInChildren<TMPro.TMP_Text>().SetText(title);

            action.Invoke(component);
        }
    }
}
