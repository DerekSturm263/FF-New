using Extensions.Components.Input;
using Extensions.Components.Miscellaneous;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Extensions.Components.Scene
{
    public class SceneController : Miscellaneous.Controller<SceneController>
    {
        private GameObject _transition;
        private string _lastScene, _nextScene;
        private bool _isTransitioning;

        private GameObject _transitionCanvas;
        private GameObject _transitionInstance;

        public void SetTransition(GameObject transition)
        {
            _transition = transition;
        }

        public void LoadScene(string sceneName)
        {
            if (_isTransitioning)
                return;

            _lastScene = SceneManager.GetActiveScene().name;
            _nextScene = sceneName;

            foreach (InputEvent inputEvent in FindObjectsByType<InputEvent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                inputEvent.enabled = false;
            }

            _isTransitioning = true;
            EventSystem.current.enabled = false;

            if (!StartTransition())
                LoadSceneNoTransition(sceneName);
        }

        private void LoadSceneNoTransition(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            _isTransitioning = false;
        }

        public void LoadLastScene(string fallback) => SceneManager.LoadScene(string.IsNullOrEmpty(_lastScene) ? fallback : _lastScene);

        public void LoadAsync()
        {
            _transitionCanvas.GetComponent<CanvasScaler>().StartCoroutine(LoadSceneWithTransition(_nextScene));
        }

        private IEnumerator LoadSceneWithTransition(string scene)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            operation.allowSceneActivation = false;

            yield return new WaitUntil(() => operation.progress >= 0.9f);

            operation.allowSceneActivation = true;
            StopTransition();
        }

        private bool StartTransition()
        {
            if (!_transitionCanvas)
                _transitionCanvas = GameObject.FindGameObjectWithTag("Transition Canvas");

            if (!_transitionCanvas)
                return false;

            if (_transition)
            {
                _transitionInstance = Instantiate(_transition, _transitionCanvas.transform);
                return true;
            }

            return false;
        }

        private void StopTransition()
        {
            if (_transitionInstance)
                _transitionInstance.GetComponent<Animator>().SetTrigger("Finished");

            _isTransitioning = false;
        }
    }
}
