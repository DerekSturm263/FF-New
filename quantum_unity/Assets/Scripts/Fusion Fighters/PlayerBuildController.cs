using Quantum;
using UnityEngine;

public class PlayerBuildController : MonoBehaviour
{
    [SerializeField] private AssetRefFFAvatar _female;
    [SerializeField] private AssetRefFFAvatar _male;

    private EntityView _entityView;

    private void Awake()
    {
        _entityView = GetComponent<EntityView>();

        QuantumEvent.Subscribe<EventOnPlayerSetAvatar>(listener: this, handler: SetBuild);
    }

    private void SetBuild(EventOnPlayerSetAvatar e)
    {
        if (e.Player.Entity != _entityView.EntityRef)
            return;

        if (e.New == _female)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (e.New == _male)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
