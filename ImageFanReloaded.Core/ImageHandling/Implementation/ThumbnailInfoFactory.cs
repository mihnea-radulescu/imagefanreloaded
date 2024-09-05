using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfoFactory : IThumbnailInfoFactory
{
	public ThumbnailInfoFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public IThumbnailInfo GetThumbnailInfo(int thumbnailSize, IImageFile imageFile)
		=> new ThumbnailInfo(_globalParameters, thumbnailSize, imageFile);
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	
	#endregion
}
