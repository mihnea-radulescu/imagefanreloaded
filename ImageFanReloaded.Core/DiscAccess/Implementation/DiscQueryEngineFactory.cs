using System;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFactory : IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(
		IGlobalParameters globalParameters, IImageFileFactory imageFileFactory, IFileSizeEngine fileSizeEngine)
	{
		_globalParameters = globalParameters;
		_imageFileFactory = imageFileFactory;
		_fileSizeEngine = fileSizeEngine;
	}

	public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_globalParameters.IsLinux)
		{
			return new LinuxDiscQueryEngine(_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		if (_globalParameters.IsWindows)
		{
			return new WindowsDiscQueryEngine(_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		if (_globalParameters.IsMacOs)
		{
			return new MacOsDiscQueryEngine(_globalParameters, _imageFileFactory, _fileSizeEngine);
		}

		throw new PlatformNotSupportedException("Operating system not supported!");
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageFileFactory _imageFileFactory;
	private readonly IFileSizeEngine _fileSizeEngine;
}
