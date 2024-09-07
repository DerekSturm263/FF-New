using Extensions.Components.Input;
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

        public void SetTransition(GameObject transition) => _transition = transition;
        public void ClearTransition() => _transition = null;

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
                Invoke(nameof(LoadSceneNoTransition), 1);
        }

        public void LoadSceneImmediate(string sceneName)
        {
            _nextScene = sceneName;
            LoadSceneNoTransition();
        }

        private void LoadSceneNoTransition()
        {
            SceneManager.LoadScene(_nextScene);
            _isTransitioning = false;
        }

        public void LoadLastScene(string fallback) => LoadScene(string.IsNullOrEmpty(_lastScene) ? fallback : _lastScene);

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

        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}
