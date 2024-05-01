using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Implementation;

public class FolderVisualStateFactory : IFolderVisualStateFactory
{
	public FolderVisualStateFactory(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IThumbnailInfoFactory thumbnailInfoFactory,
		IDiscQueryEngine discQueryEngine)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_thumbnailInfoFactory = thumbnailInfoFactory;
		_discQueryEngine = discQueryEngine;
	}
	
	public IFolderVisualState GetFolderVisualState(
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath)
    {
		IFolderVisualState folderVisualState = new FolderVisualState(
			_globalParameters,
			_fileSizeEngine,
			_thumbnailInfoFactory,
			_discQueryEngine,
			contentTabItem,
			folderName,
			folderPath);

		return folderVisualState;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;

	#endregion
}
