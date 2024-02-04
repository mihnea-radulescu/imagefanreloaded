using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Presenters;

public class MainPresenter
{
	public MainPresenter(
		IDiscQueryEngine discQueryEngine,
		IVisualActionDispatcher dispatcher,
		IFolderVisualStateFactory folderVisualStateFactory,
		IImageViewFactory imageViewFactory,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_dispatcher = dispatcher;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;

		_mainView = mainView;
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
	}

	#region Private

	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IVisualActionDispatcher _dispatcher;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;

	private readonly IMainView _mainView;

	private void OnContentTabItemAdded(object sender, TabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		contentTabItem.ImageViewFactory = _imageViewFactory;
		contentTabItem.GenerateThumbnailsLockObject = new object();

		contentTabItem.FolderChanged += OnFolderChanged;

		PopulateDrivesAndSpecialFolders(contentTabItem);
	}

	private void PopulateDrivesAndSpecialFolders(IContentTabItem contentTabItem)
	{
		var specialFolders = _discQueryEngine.GetUserFolders();
		var drives = _discQueryEngine.GetDrives();

		contentTabItem.PopulateSubFoldersTree(specialFolders, true);
		contentTabItem.PopulateSubFoldersTree(drives, true);
	}

	private void OnFolderChanged(object sender, FolderChangedEventArgs e)
	{
		var contentTabItem = (IContentTabItem)sender;
		var path = e.Path;

		UpdateUserInterface(contentTabItem, path);
	}

	private void UpdateUserInterface(IContentTabItem contentTabItem, string folderPath)
	{
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			_discQueryEngine,
			_dispatcher,
			contentTabItem,
			folderPath);

		contentTabItem.FolderVisualState.UpdateVisualState();
	}

	#endregion
}
