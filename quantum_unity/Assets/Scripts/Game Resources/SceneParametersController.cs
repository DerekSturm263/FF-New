using Extensions.Components.Miscellaneous;

public class SceneParametersController : Controller<SceneParametersController>
{
    private string _parameter;
    public string Parameter => _parameter;
    public void SetParameter(string parameter) => _parameter = parameter;
}
