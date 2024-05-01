using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;

namespace ImageFanReloaded.Core;

public class MainPresenter
{
	public MainPresenter(
		IDiscQueryEngine discQueryEngine,
		IFolderVisualStateFactory folderVisualStateFactory,
		IImageViewFactory imageViewFactory,
		IMainView mainView,
		IInputPathContainer inputPathContainer)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;

		_mainView = mainView;
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;

		_inputPathContainer = inputPathContainer;
	}

	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;

	private readonly IMainView _mainView;

	private readonly IInputPathContainer _inputPathContainer;

	private async void OnContentTabItemAdded(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		contentTabItem.ImageViewFactory = _imageViewFactory;
		contentTabItem.GenerateThumbnailsLockObject = new object();
		contentTabItem.FolderChanged += OnFolderChanged;

		var rootFolders = await PopulateRootFolders(contentTabItem);
		PopulateInputPath(contentTabItem, rootFolders);
	}
	
	private void OnContentTabItemClosed(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		contentTabItem.FolderChanged -= OnFolderChanged;

		ClearContentTabItem(contentTabItem);
	}
	
	private async void OnFolderChanged(object? sender, FolderChangedEventArgs e)
	{
		var contentTabItem = (IContentTabItem)sender!;
		var name = e.Name;
		var path = e.Path;

		await UpdateContentTabItem(contentTabItem, name, path);
	}

	private async Task<IReadOnlyCollection<FileSystemEntryInfo>> PopulateRootFolders(IContentTabItem contentTabItem)
	{
		var rootFolders = await _discQueryEngine.GetRootFolders();
		
		contentTabItem.PopulateSubFoldersTree(rootFolders, true);

		return rootFolders;
	}

	private async Task UpdateContentTabItem(IContentTabItem contentTabItem, string folderName, string folderPath)
	{
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			folderName,
			folderPath);

		await contentTabItem.FolderVisualState.UpdateVisualState();
	}
	
	private static void ClearContentTabItem(IContentTabItem contentTabItem)
	{
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState?.ClearVisualState();
	}

	private void PopulateInputPath(
		IContentTabItem contentTabItem, IReadOnlyCollection<FileSystemEntryInfo> rootFolders)
	{
		if (!_inputPathContainer.ShouldPopulateInputPath())
		{
			return;
		}
		
		_inputPathContainer.HasPopulatedInputPath = true;
	}

	#endregion
}
