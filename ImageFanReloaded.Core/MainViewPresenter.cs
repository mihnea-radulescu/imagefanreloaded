using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;

namespace ImageFanReloaded.Core;

public class MainViewPresenter
{
	public MainViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IFolderVisualStateFactory folderVisualStateFactory,
		IImageViewFactory imageViewFactory,
		ITabOptionsViewFactory tabOptionsViewFactory,
		IAboutViewFactory aboutViewFactory,
		IImageInfoViewFactory imageInfoViewFactory,
		IInputPathHandlerFactory inputPathHandlerFactory,
		IInputPathHandler commandLineArgsInputPathHandler,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;
		_tabOptionsViewFactory = tabOptionsViewFactory;
		_aboutViewFactory = aboutViewFactory;
		_imageInfoViewFactory = imageInfoViewFactory;
		_inputPathHandlerFactory = inputPathHandlerFactory;
		
		_commandLineArgsInputPathHandler = commandLineArgsInputPathHandler;
		_shouldProcessCommandLineArgsInputPath = true;

		_mainView = mainView;
		
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;
	}

	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;
	private readonly ITabOptionsViewFactory _tabOptionsViewFactory;
	private readonly IAboutViewFactory _aboutViewFactory;
	private readonly IImageInfoViewFactory _imageInfoViewFactory;
	private readonly IInputPathHandlerFactory _inputPathHandlerFactory;
	
	private readonly IInputPathHandler _commandLineArgsInputPathHandler;
	private bool _shouldProcessCommandLineArgsInputPath;

	private readonly IMainView _mainView;

	private async void OnContentTabItemAdded(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		contentTabItem.ImageViewFactory = _imageViewFactory;

		var rootFolders = await PopulateRootFolders(contentTabItem);

		var shouldProcessInputPath = _shouldProcessCommandLineArgsInputPath &&
									 _commandLineArgsInputPathHandler.CanProcessInputPath();
		if (shouldProcessInputPath)
		{
			_shouldProcessCommandLineArgsInputPath = false;
			
			await BuildInputFolderTreeView(contentTabItem, rootFolders, _commandLineArgsInputPathHandler);
			
			EnableContentTabEventHandling(contentTabItem);
			
			contentTabItem.RaiseFolderChangedEvent();
		}
		else
		{
			EnableContentTabEventHandling(contentTabItem);
			
			contentTabItem.SetFocusOnSelectedFolderTreeViewItem();
		}
	}
	
	private void OnContentTabItemClosed(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();
		contentTabItem.FolderVisualState?.ClearVisualState();
		
		DisableContentTabEventHandling(contentTabItem);
		
		contentTabItem.FolderChangedMutex!.Dispose();
	}
	
	private async void OnTabOptionsRequested(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		var tabOptionsView = _tabOptionsViewFactory.GetTabOptionsView(contentTabItem);
		
		tabOptionsView.TabOptionsChanged += OnTabOptionsChanged;
		await contentTabItem.ShowTabOptions(tabOptionsView);
		tabOptionsView.TabOptionsChanged -= OnTabOptionsChanged;
	}

	private async void OnAboutInfoRequested(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		var aboutView = _aboutViewFactory.GetAboutView();
		await contentTabItem.ShowAboutInfo(aboutView);
	}

	private async void OnImageInfoRequested(object? sender, ImageSelectedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var imageFile = e.ImageFile;
		
		var imageInfoView = _imageInfoViewFactory.GetImageInfoView(imageFile);
		await contentTabItem.ShowImageInfo(imageInfoView);
	}

	private static async void OnTabOptionsChanged(object? sender, TabOptionsChangedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var tabOptionChanges = e.TabOptionChanges;

		var shouldRaiseFolderChangedEvent =
			tabOptionChanges.HasChangedThumbnailSize ||
			tabOptionChanges.HasChangedRecursiveFolderBrowsing;

		var shouldRaiseFolderOrderingChangedEvent = tabOptionChanges.HasChangedFolderOrdering;

		var shouldRaisePanelsSplittingRatioChangedEvent =
			tabOptionChanges.HasChangedPanelsSplittingRatio;

		var shouldSaveAsDefault = tabOptionChanges.ShouldSaveAsDefault;

		if (shouldRaiseFolderChangedEvent)
		{
			contentTabItem.RaiseFolderChangedEvent();
		}

		if (shouldRaiseFolderOrderingChangedEvent)
		{
			contentTabItem.RaiseFolderOrderingChangedEvent();
		}

		if (shouldRaisePanelsSplittingRatioChangedEvent)
		{
			contentTabItem.RaisePanelsSplittingRatioChangedEvent();
		}

		if (shouldSaveAsDefault)
		{
			await contentTabItem.TabOptions!.SaveDefaultTabOptions();
		}
	}
	
	private async void OnFolderChanged(object? sender, FolderChangedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			e.Name,
			e.Path);

		await contentTabItem.FolderVisualState.UpdateVisualState(contentTabItem.TabOptions!);
	}
	
	private async void OnFolderOrderingChanged(object? sender, FolderOrderingChangedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		var isExpandedFolderTreeViewSelectedItem = contentTabItem.GetFolderTreeViewSelectedItemExpandedState()
			?? false;

		DisableContentTabEventHandling(contentTabItem);

		var rootFolders = await PopulateRootFolders(contentTabItem);

		var folderChangedInputPathHandler = _inputPathHandlerFactory.GetInputPathHandler(e.Path);
		await BuildInputFolderTreeView(contentTabItem, rootFolders, folderChangedInputPathHandler);

		EnableContentTabEventHandling(contentTabItem);

		contentTabItem.SetFocusOnSelectedFolderTreeViewItem();
		contentTabItem.SetFolderTreeViewSelectedItemExpandedState(isExpandedFolderTreeViewSelectedItem);
	}

	private async Task<IReadOnlyList<FileSystemEntryInfo>> PopulateRootFolders(IContentTabItem contentTabItem)
	{
		await _discQueryEngine.BuildSkipRecursionFolderPaths();
		var rootFolders = await _discQueryEngine.GetRootFolders();
		
		contentTabItem.PopulateRootNodesSubFoldersTree(rootFolders);

		return rootFolders;
	}
	
	private async Task BuildInputFolderTreeView(
		IContentTabItem contentTabItem,
		IReadOnlyList<FileSystemEntryInfo> rootFolders,
		IInputPathHandler inputPathHandler)
	{
		FileSystemEntryInfo? matchingFileSystemEntryInfo;
		var subFolders = rootFolders;
		var startAtRootFolders = true;

		do
		{
			matchingFileSystemEntryInfo = await inputPathHandler.GetMatchingFileSystemEntryInfo(subFolders);

			if (matchingFileSystemEntryInfo is not null)
			{
				contentTabItem.SaveMatchingTreeViewItem(matchingFileSystemEntryInfo, startAtRootFolders);
				startAtRootFolders = false;
				
				subFolders = await _discQueryEngine.GetSubFolders(
					matchingFileSystemEntryInfo.Path, contentTabItem.TabOptions!.FileSystemEntryInfoOrdering);
				contentTabItem.PopulateSubFoldersTreeOfParentTreeViewItem(subFolders);
			}
		} while (matchingFileSystemEntryInfo is not null);
	}

	private void EnableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.EnableFolderTreeViewSelectedItemChanged();

		contentTabItem.FolderChanged += OnFolderChanged;
		contentTabItem.FolderOrderingChanged += OnFolderOrderingChanged;

		contentTabItem.TabOptionsRequested += OnTabOptionsRequested;
		contentTabItem.AboutInfoRequested += OnAboutInfoRequested;
		contentTabItem.ImageInfoRequested += OnImageInfoRequested;
	}

	private void DisableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.DisableFolderTreeViewSelectedItemChanged();

		contentTabItem.FolderChanged -= OnFolderChanged;
		contentTabItem.FolderOrderingChanged -= OnFolderOrderingChanged;

		contentTabItem.TabOptionsRequested -= OnTabOptionsRequested;
		contentTabItem.AboutInfoRequested -= OnAboutInfoRequested;
		contentTabItem.ImageInfoRequested -= OnImageInfoRequested;
	}

	#endregion
}
