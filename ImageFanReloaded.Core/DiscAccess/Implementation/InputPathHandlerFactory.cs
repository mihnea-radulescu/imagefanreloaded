using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class InputPathHandlerFactory : IInputPathHandlerFactory
{
	public InputPathHandlerFactory(
		IGlobalParameters globalParameters,
		ITabOptions tabOptions)
	{
		_globalParameters = globalParameters;
		_tabOptions = tabOptions;
	}

	public IInputPathHandler GetInputPathHandler(string? inputPath)
		=> new InputPathHandler(_globalParameters, _tabOptions, inputPath);

	private readonly IGlobalParameters _globalParameters;
	private readonly ITabOptions _tabOptions;
}
