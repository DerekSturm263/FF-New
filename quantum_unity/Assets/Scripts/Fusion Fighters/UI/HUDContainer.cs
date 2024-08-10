using UnityEngine;

public class HUDContainer : MonoBehaviour
{
    [SerializeField] private HUDPlayerLink[] _links;
    public HUDPlayerLink[] Links => _links;
}
