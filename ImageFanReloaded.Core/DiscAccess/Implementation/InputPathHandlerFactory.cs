using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathHandlerFactory : IInputPathHandlerFactory
{
    public InputPathHandlerFactory(IGlobalParameters globalParameters, IDiscQueryEngine discQueryEngine)
    {
        _globalParameters = globalParameters;
        _discQueryEngine = discQueryEngine;
    }

    public IInputPathHandler GetInputPathHandler(string? inputPath)
        => new InputPathHandler(_globalParameters, _discQueryEngine, inputPath);
    
    #region Private
    
    private readonly IGlobalParameters _globalParameters;
    private readonly IDiscQueryEngine _discQueryEngine;
    
    #endregion
}
