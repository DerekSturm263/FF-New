using Photon.Deterministic;
using Quantum;
using UnityEngine.Events;

public abstract partial class ItemAsset : InfoAssetAsset
{
    public Extensions.Types.Dictionary<string, Clip> SFX;
    public VFX ItemVFX;
    public VFX UserVFX;

    public UnityEvent<QuantumGame, EntityView, (EntityView, ItemAsset, FPVector2)> OnUse;
}
