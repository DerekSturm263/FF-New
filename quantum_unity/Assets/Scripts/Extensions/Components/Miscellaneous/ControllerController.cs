using UnityEngine;

namespace Extensions.Components.Miscellaneous
{
    public class ControllerController : MonoBehaviour
    {
        public void Initialize()
        {
            foreach (ControllerBase controller in GetComponents<ControllerBase>())
            {
                controller.Initialize();
            }
        }

        public void UpdateDt()
        {
            foreach (ControllerBase controller in GetComponents<ControllerBase>())
            {
                controller.UpdateDt(UnityEngine.Time.deltaTime);
            }
        }

        public void UpdateDtFixed()
        {
            foreach (ControllerBase controller in GetComponents<ControllerBase>())
            {
                controller.UpdateDt(UnityEngine.Time.fixedDeltaTime);
            }
        }

        public void Shutdown()
        {
            foreach (ControllerBase controller in GetComponents<ControllerBase>())
            {
                controller.Shutdown();
            }
        }
    }
}
