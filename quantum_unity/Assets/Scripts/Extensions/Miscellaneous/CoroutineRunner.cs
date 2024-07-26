using Extensions.Components.Miscellaneous;
using System.Collections;

public class CoroutineRunner : Controller<CoroutineRunner>
{
    private void Awake()
    {
        Initialize();
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
