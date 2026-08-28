using ImageFanReloaded.Core.Controls.Implementation;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls.Factories.Implementation;

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
		IFileSystemEntryInfo fileSystemEntryInfo)
	{
		IFolderVisualState folderVisualState = new FolderVisualState(
			_globalParameters,
			_fileSizeEngine,
			_thumbnailInfoFactory,
			_discQueryEngine,
			contentTabItem,
			fileSystemEntryInfo);

		return folderVisualState;
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;
}
