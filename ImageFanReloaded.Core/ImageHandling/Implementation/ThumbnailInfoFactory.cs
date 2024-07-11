using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfoFactory : IThumbnailInfoFactory
{
	public ThumbnailInfoFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public IThumbnailInfo GetThumbnailInfo(IImageFile imageFile)
		=> new ThumbnailInfo(_globalParameters, imageFile);
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	
	#endregion
}
