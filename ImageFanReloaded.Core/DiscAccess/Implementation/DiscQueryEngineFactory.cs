using System;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFactory : IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_imageFileFactory = imageFileFactory;
	}

	public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_globalParameters.IsLinux)
		{
			return new LinuxDiscQueryEngine(_globalParameters, _fileSizeEngine, _imageFileFactory);
		}
		
		if (_globalParameters.IsWindows)
		{
			return new WindowsDiscQueryEngine(_globalParameters, _fileSizeEngine, _imageFileFactory);
		}
		
		if (_globalParameters.IsMacOS)
		{
			return new MacOSDiscQueryEngine(_globalParameters, _fileSizeEngine, _imageFileFactory);
		}

		throw new PlatformNotSupportedException("Operating system not supported!");
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IImageFileFactory _imageFileFactory;

	#endregion
}