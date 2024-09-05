using UnityEngine;

namespace Extensions.Components.Miscellaneous
{
    public class MiscellaneousController : Controller<MiscellaneousController>
    {
        public void DisableAnimationComponent(GameObject gameObject) => gameObject.GetComponent<Animator>().enabled = false;

        public void ToggleGameObject(GameObject gameObject) => gameObject.SetActive(!gameObject.activeSelf);

        public void Destroy(GameObject gameObject) => Object.Destroy(gameObject);

        public void Enable(GameObject gameObject) => gameObject.SetActive(true);
        public void Disable(GameObject gameObject) => gameObject.SetActive(false);

        public void QuitGame() => Application.Quit();

        public void DebugLog(string message) => Debug.Log(message);

        public void DontDestroyOnLoad(GameObject gameObject)
        {
            Object.DontDestroyOnLoad(gameObject);
        }

        public void DestroyMainMenuCanvas()
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Main Menu Canvas");
            Object.Destroy(canvas);
        }

        public void ResetTrigger(GameObject gameObj)
        {
            gameObj.GetComponent<Animator>().ResetTrigger("On");
            gameObj.GetComponent<Animator>().ResetTrigger("Off");
        }

        public void OpenURL(string link)
        {
            Application.OpenURL(link);
        }
    }
}
