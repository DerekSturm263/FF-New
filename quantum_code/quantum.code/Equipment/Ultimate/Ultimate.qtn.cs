using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Ultimate : InfoAsset
    {
        public AssetRefCameraSettings CameraSettings;
        public FP CameraSettingsTime;

        public AssetRefQuantumAnimationEvent Move;
        public int Length;

        public virtual void OnBegin(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            f.Events.OnSetCameraSettings(CameraSettings, CameraSettingsTime);
        }

        public abstract void OnEnd(Frame f, ref CharacterControllerSystem.Filter filter);
    }
}
