using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Factories.Implementation;

public class ThumbnailInfoFactory : IThumbnailInfoFactory
{
	public ThumbnailInfoFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public IThumbnailInfo GetThumbnailInfo(
		ITabOptions tabOptions, IImageFile imageFile)
			=> new ThumbnailInfo(_globalParameters, tabOptions, imageFile);

	private readonly IGlobalParameters _globalParameters;
}
