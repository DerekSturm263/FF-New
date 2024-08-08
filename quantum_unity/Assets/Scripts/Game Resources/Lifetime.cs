using Extensions.Miscellaneous;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime;
    [SerializeField] private bool _useScaledTime = true;

    private void Awake()
    {
        if (_useScaledTime)
            Invoke(nameof(DestroySelf), _lifetime);
        else
            Helper.Delay(_lifetime, DestroySelf);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
