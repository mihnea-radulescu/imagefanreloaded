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
	}

    public IDiscQueryEngine GetDiscQueryEngine()
	{
		if (_isWindows)
		{
			return new WindowsDiscQueryEngine(_imageFileFactory);
		}

		return new UnixDiscQueryEngine(_imageFileFactory);
	}

	#region Private

	private readonly IImageFileFactory _imageFileFactory;

	private readonly bool _isWindows;

	#endregion
}
