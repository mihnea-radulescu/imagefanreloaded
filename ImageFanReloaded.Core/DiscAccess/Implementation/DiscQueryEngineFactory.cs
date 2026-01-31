using ImageFanReloaded.Core.Exceptions;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFactory : IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory,
		IFileSizeEngine fileSizeEngine)
	{
		_globalParameters = globalParameters;
		_imageFileFactory = imageFileFactory;
		_fileSizeEngine = fileSizeEngine;
	}

	public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.Linux or RuntimeEnvironmentType.LinuxFlatpak)
		{
			return new LinuxDiscQueryEngine(
				_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.Windows)
		{
			return new WindowsDiscQueryEngine(
				_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		if (_globalParameters.RuntimeEnvironmentType ==
			RuntimeEnvironmentType.MacOs)
		{
			return new MacOsDiscQueryEngine(
				_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		throw new RuntimeEnvironmentNotSupportedException();
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageFileFactory _imageFileFactory;
	private readonly IFileSizeEngine _fileSizeEngine;
}
