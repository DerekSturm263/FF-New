using Extensions.Components.Miscellaneous;

public class HUDController : Controller<HUDController>
{
    private void Awake()
    {
        Initialize();
    }

    public void Enable()
    {
        _instance.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _instance.gameObject.SetActive(false);
    }
}
