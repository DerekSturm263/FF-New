using System.Collections.Generic;
using UnityEngine;

namespace GameResources.Audio
{
    /// <summary>
    /// Exposes methods that allow any Action to be executed based on Audio Events instead of immediately.
    /// </summary>
    public class TrackEventController : Extensions.Components.Miscellaneous.Controller<TrackEventController>
    {
        private readonly List<System.Action> _nextAction = new();
        private readonly List<System.Action> _everyAction = new();

        [SerializeField] private Extensions.Types.ReturningUnityEvent<int> _getBPM;

        private int _frame;

        public override void Initialize()
        {
            // Make sure to call the base class's initialize.
            base.Initialize();
        }

        public override void UpdateDt(float dt)
        {
            // Increment the current frame. This should happen exactly 60 times every second.
            ++_frame;

            // Set the sounds per minute to be based on the dt.
            int spm = (int)(60 / dt);

            // Only invoke events if the current frame is the start of the next beat (based on the BPM of the current Track).
            if (_frame % (spm / _getBPM.Invoke()) == 0)
            {
                Debug.Log("Beat");

                // Execute every event that's only supposed to happen this beat, then erase the list.
                foreach (System.Action action in _nextAction)
                {
                    action.Invoke();
                }
                _nextAction.Clear();

                // Execute every event that's supposed to happen every beat.
                foreach (System.Action action in  _everyAction)
                {
                    action.Invoke();
                }
            }
        }

        public void ResetFrame()
        {
            _frame = 0;
        }

        /// <summary>
        /// Delays an Action to be invoked on the next beat of the current Track.
        /// </summary>
        /// <param name="action">The Action to be invoked.</param>
        public void ExecuteOnNextBeat(System.Action action)
        {
            // Add the Action to the list of Actions to invoke on the next beat.
            _nextAction.Add(action);
        }

        /// <summary>
        /// Delays an InvokeableGameObject to be invoked on the next beat of the current Track.
        /// </summary>
        /// <param name="gameObject">The InvokeableGameObject to be invoked.</param>
        public void ExecuteEventOnNextBeat(Extensions.Components.Miscellaneous.InvokableGameObject gameObject) => ExecuteOnNextBeat(() => gameObject.Invoke());

        /// <summary>
        /// Set an Action to be invoked on every beat of the current Track.
        /// </summary>
        /// <param name="action">The Action to be invoked.</param>
        public void ExecuteOnEveryBeat(System.Action action)
        {
            // Add the Action to the list of Actions to invoke on every beat.
            _everyAction.Add(action);
        }

        /// <summary>
        /// Set an InvokeableGameObject to be invoked on every beat of the current Track.
        /// </summary>
        /// <param name="gameObject">The InvokeableGameObject to be invoked.</param>
        public void ExecuteEventOnEveryBeat(Extensions.Components.Miscellaneous.InvokableGameObject gameObject) => ExecuteOnEveryBeat(() => gameObject.Invoke());
    }
}
