using ImageFanReloaded.Core.Exceptions;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFactory : IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(
		IGlobalParameters globalParameters, IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_imageFileFactory = imageFileFactory;
	}

	public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.Linux or RuntimeEnvironmentType.LinuxFlatpak)
		{
			return new LinuxDiscQueryEngine(
				_globalParameters, _imageFileFactory);
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.Windows)
		{
			return new WindowsDiscQueryEngine(
				_globalParameters, _imageFileFactory);
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.MacOs)
		{
			return new MacOsDiscQueryEngine(
				_globalParameters, _imageFileFactory);
		}

		throw new RuntimeEnvironmentNotSupportedException();
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageFileFactory _imageFileFactory;
}
