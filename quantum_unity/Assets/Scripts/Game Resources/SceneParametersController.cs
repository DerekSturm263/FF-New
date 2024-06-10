using Extensions.Components.Miscellaneous;
using System;

public class SceneParametersController : Controller<SceneParametersController>
{
    [NonSerialized] private string _parameter;
    public string Parameter => _parameter;
    public void SetParameter(string parameter) => _parameter = parameter;
}
