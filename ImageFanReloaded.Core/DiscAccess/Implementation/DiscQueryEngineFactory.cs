using System;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class DiscQueryEngineFactory : IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory)
    {
	    _globalParameters = globalParameters;
	    _imageFileFactory = imageFileFactory;

		_isWindows = OperatingSystem.IsWindows();
		_isLinux = OperatingSystem.IsLinux();
		_isMacOS = OperatingSystem.IsMacOS();
	}

    public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_isWindows)
		{
			return new WindowsDiscQueryEngine(_globalParameters, _imageFileFactory);
		}

		if (_isLinux)
		{
			return new LinuxDiscQueryEngine(_globalParameters, _imageFileFactory);
		}

		if (_isMacOS)
		{
			return new MacOSDiscQueryEngine(_globalParameters, _imageFileFactory);
		}

		throw new PlatformNotSupportedException("Operating system not supported!");
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IImageFileFactory _imageFileFactory;

	private readonly bool _isWindows;
	private readonly bool _isLinux;
	private readonly bool _isMacOS;

	#endregion
}
