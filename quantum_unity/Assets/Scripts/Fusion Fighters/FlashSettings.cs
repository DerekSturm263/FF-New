using UnityEngine;

[CreateAssetMenu(fileName = "New Flash Settings", menuName = "Fusion Fighters/Flash Settings")]
public class FlashSettings : ScriptableObject
{
    [SerializeField] private Extensions.Types.Dictionary<string, Extensions.Types.Tuple<AnimationCurve, float>> _curves;

    public void Apply(Material material, float t)
    {
        foreach (var kvp in _curves)
        {
            material.SetFloat(kvp.Key, kvp.Value.Item1.Evaluate(t) * kvp.Value.Item2);
        }
    }
}
