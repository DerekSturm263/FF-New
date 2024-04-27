using Extensions.Types;
using Quantum;
using Quantum.Collections;
using UnityEngine;

public class GetHurtboxesCallback : QuantumCallbacks
{
    [SerializeField] private EntityView _player;
    [SerializeField] private Dictionary<Quantum.HurtboxType, Transform> _points;

    public override void OnUpdateView(QuantumGame game)
    {
        Frame f = game.Frames.Predicted;

        if (!_player.EntityRef.IsValid)
            return;

        if (f.TryGet(_player.EntityRef, out Stats stats))
        {
            QEnumDictionary<HurtboxType, EntityRef> hurtboxes = f.ResolveDictionary(stats.Hurtboxes);
            
            foreach (var kvp in _points)
            {
                //hurtboxes[kvp.Key] = kvp.Value;
            }
        }
    }
}
