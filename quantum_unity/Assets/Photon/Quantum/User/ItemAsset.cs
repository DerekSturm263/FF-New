using Photon.Deterministic;
using Quantum;
using UnityEngine.Events;

public partial class ItemAsset : AssetBase
{
    public Clip SFX;
    public Extensions.Types.Dictionary<string, VFX> VFX;

    public UnityEvent<QuantumGame, EntityView, (EntityView, ItemAsset, FPVector2)> OnUse;
}
