using Extensions.Components.Miscellaneous;

public class HUDController : Controller<HUDController>
{
    private void Awake()
    {
        Initialize();
    }

    public void Enable()
    {
        if (_instance)
            _instance.gameObject.SetActive(true);
    }

    public void Disable()
    {
        if (_instance)
            _instance.gameObject.SetActive(false);
    }
}
