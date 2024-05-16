using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private void Awake()
    {
        Invoke(nameof(DestroySelf), _lifetime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
