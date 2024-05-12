using Quantum;
using UnityEngine;

public class ItemEventListener : MonoBehaviour
{
    private EntityViewUpdater _entityViewUpdater;

    private void Awake()
    {
        _entityViewUpdater = FindFirstObjectByType<EntityViewUpdater>();

        QuantumEvent.Subscribe<EventOnItemUse>(listener: this, handler: e =>
        {
            e.ItemAsset.GetUnityAsset().OnUse.Invoke(e.Game, _entityViewUpdater.GetView(e.Player.Entity), (_entityViewUpdater.GetView(e.ItemEntity), e.ItemAsset.GetUnityAsset(), e.Position));
        });
    }
}
