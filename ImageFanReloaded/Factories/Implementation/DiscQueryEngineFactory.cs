using System;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.Disc.Implementation;

namespace ImageFanReloaded.Factories.Implementation;

public class DiscQueryEngineFactory
	: IDiscQueryEngineFactory
{
	public DiscQueryEngineFactory(IImageFileFactory imageFileFactory)
    {
		_imageFileFactory = imageFileFactory;

		_isWindows = OperatingSystem.IsWindows();
		_isLinux = OperatingSystem.IsLinux();
		_isMacOS = OperatingSystem.IsMacOS();
	}

    public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_isWindows)
		{
			return new WindowsDiscQueryEngine(_imageFileFactory);
		}

		if (_isLinux)
		{
			return new LinuxDiscQueryEngine(_imageFileFactory);
		}

		if (_isMacOS)
		{
			return new MacOSDiscQueryEngine(_imageFileFactory);
		}

		throw new PlatformNotSupportedException("Operating system not supported!");
	}

	#region Private

	private readonly IImageFileFactory _imageFileFactory;

	private readonly bool _isWindows;
	private readonly bool _isLinux;
	private readonly bool _isMacOS;

	#endregion
}
