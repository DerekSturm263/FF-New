using UnityEngine;

namespace Extensions.Components.Miscellaneous
{
    public abstract class ControllerBase : MonoBehaviour
    {
        public abstract void Initialize();
        public virtual void UpdateDt(float dt) { }
        public abstract void Shutdown();
    }

    public class Controller<TSelf> : ControllerBase where TSelf : ControllerBase
    {
        protected static TSelf _instance;
        /// <summary>
        /// The current instance of this Controller.
        /// </summary>
        public static TSelf Instance => _instance;

        public override void Initialize()
        {
            Debug.Log($"Initialized {typeof(TSelf).Name}");

            _instance = this as TSelf;
        }

        public override void Shutdown()
        {
            _instance = null;
            
            Debug.Log($"Shut down {typeof(TSelf).Name}");
        }
    }
}
