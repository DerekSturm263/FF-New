public class ManualCameraController : MonoBehaviour
{
        [SerializeField] private CameraSettingsAsset _settings;
        public void SetCameraSettings(CameraSettingsAsset settings) => _instance._settings = settings;
        
        private UnityEngine.Camera _cam;
        public UnityEngine.Camera Cam => _cam;

        private void LateUpdate()
        {
                if (!_settings)
                        return;

                
        }
}
