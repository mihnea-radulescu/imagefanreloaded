using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Implementation;

public class FolderVisualStateFactory : IFolderVisualStateFactory
{
	public FolderVisualStateFactory(
		IGlobalParameters globalParameters,
		IThumbnailInfoFactory thumbnailInfoFactory,
		IDiscQueryEngine discQueryEngine,
		IDispatcher dispatcher)
	{
		_globalParameters = globalParameters;
		_thumbnailInfoFactory = thumbnailInfoFactory;
		_discQueryEngine = discQueryEngine;
		_dispatcher = dispatcher;
	}
	
	public IFolderVisualState GetFolderVisualState(
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath)
    {
		IFolderVisualState folderVisualState = new FolderVisualState(
			_globalParameters,
			_thumbnailInfoFactory,
			_discQueryEngine,
			_dispatcher,
			contentTabItem,
			folderName,
			folderPath);

		return folderVisualState;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IDispatcher _dispatcher;
	
	#endregion
}
