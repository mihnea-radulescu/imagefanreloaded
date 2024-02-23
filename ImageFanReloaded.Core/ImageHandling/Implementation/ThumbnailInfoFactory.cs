using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Global;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ThumbnailInfoFactory : IThumbnailInfoFactory
{
	public ThumbnailInfoFactory(
		IGlobalParameters globalParameters,
		IDispatcher dispatcher)
	{
		_globalParameters = globalParameters;
		_dispatcher = dispatcher;
	}

	public IThumbnailInfo GetThumbnailInfo(IImageFile imageFile)
		=> new ThumbnailInfo(_globalParameters, _dispatcher, imageFile);
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	private readonly IDispatcher _dispatcher;
	
	#endregion
}
