using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathHandlerFactory : IInputPathHandlerFactory
{
	public InputPathHandlerFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public IInputPathHandler GetInputPathHandler(string? inputPath)
		=> new InputPathHandler(_globalParameters, inputPath);

	private readonly IGlobalParameters _globalParameters;
}
