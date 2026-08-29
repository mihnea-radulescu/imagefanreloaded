using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core;

public class MainViewPresenter
{
	public MainViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IFolderVisualStateFactory folderVisualStateFactory,
		IDatabaseLogic databaseLogic,
		IImageFileFactory imageFileFactory,
		IImageViewFactory imageViewFactory,
		IImageInfoViewFactory imageInfoViewFactory,
		IImageEditViewFactory imageEditViewFactory,
		ITabOptionsViewFactory tabOptionsViewFactory,
		IThumbnailCacheOptionsViewFactory thumbnailCacheOptionsViewFactory,
		IAboutViewFactory aboutViewFactory,
		IInputPathHandlerFactory inputPathHandlerFactory,
		IInputPathHandler commandLineArgsInputPathHandler,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_databaseLogic = databaseLogic;
		_imageFileFactory = imageFileFactory;

		_imageViewFactory = imageViewFactory;
		_imageInfoViewFactory = imageInfoViewFactory;
		_imageEditViewFactory = imageEditViewFactory;
		_tabOptionsViewFactory = tabOptionsViewFactory;
		_thumbnailCacheOptionsViewFactory = thumbnailCacheOptionsViewFactory;
		_aboutViewFactory = aboutViewFactory;

		_inputPathHandlerFactory = inputPathHandlerFactory;
		_commandLineArgsInputPathHandler = commandLineArgsInputPathHandler;
		_shouldProcessCommandLineArgsInputPath = true;

		_mainView = mainView;

		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;
	}

	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IDatabaseLogic _databaseLogic;
	private readonly IImageFileFactory _imageFileFactory;

	private readonly IImageViewFactory _imageViewFactory;
	private readonly IImageInfoViewFactory _imageInfoViewFactory;
	private readonly IImageEditViewFactory _imageEditViewFactory;
	private readonly ITabOptionsViewFactory _tabOptionsViewFactory;
	private readonly IThumbnailCacheOptionsViewFactory
		_thumbnailCacheOptionsViewFactory;
	private readonly IAboutViewFactory _aboutViewFactory;

	private readonly IInputPathHandlerFactory _inputPathHandlerFactory;
	private readonly IInputPathHandler _commandLineArgsInputPathHandler;
	private bool _shouldProcessCommandLineArgsInputPath;

	private readonly IMainView _mainView;

	private async void OnContentTabItemAdded(
		object? sender, ContentTabItemAddedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var fileSystemEntryInfoToClone = e.FileSystemEntryInfoToClone;
		var shouldCloneActiveTab = e.ShouldCloneActiveTab;
		var isExpandedFolderTreeViewSelectedItem =
			e.IsExpandedFolderTreeViewSelectedItem;

		contentTabItem.ImageViewFactory = _imageViewFactory;

		var rootFolders = await PopulateRootFolders(contentTabItem);

		if (_shouldProcessCommandLineArgsInputPath)
		{
			var inputPathHandler = _shouldProcessCommandLineArgsInputPath
				? _commandLineArgsInputPathHandler
				: _inputPathHandlerFactory.GetInputPathHandler(
					fileSystemEntryInfoToClone?.QualifiedPath);

			var shouldProcessInputPath =
				_shouldProcessCommandLineArgsInputPath &&
			    inputPathHandler.CanHandleInputPath();

			if (shouldProcessInputPath)
			{
				await BuildFolderTreeViewFromInputPath(
					contentTabItem,
					inputPathHandler,
					rootFolders,
					isExpandedFolderTreeViewSelectedItem);

				EnableContentTabEventHandling(contentTabItem);

				contentTabItem.RaiseFolderContentChangedEvent();
			}
			else
			{
				EnableContentTabEventHandling(contentTabItem);

				contentTabItem.SetFocusOnSelectedFolderTreeViewItem();
			}

			_shouldProcessCommandLineArgsInputPath = false;
		}
		else if (shouldCloneActiveTab)
		{
			await BuildFolderTreeViewFromActiveTab(
				contentTabItem,
				fileSystemEntryInfoToClone!,
				isExpandedFolderTreeViewSelectedItem);

			EnableContentTabEventHandling(contentTabItem);

			contentTabItem.RaiseFolderContentChangedEvent();
		}
		else
		{
			EnableContentTabEventHandling(contentTabItem);

			contentTabItem.SetFocusOnSelectedFolderTreeViewItem();
		}
	}

	private async void OnContentTabItemClosed(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		await ClearContentTabItem(contentTabItem);
	}

	private async void OnImageInfoRequested(
		object? sender, ImageSelectedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var imageFile = e.ImageFile;

		var imageInfoView = _imageInfoViewFactory.GetImageInfoView(imageFile);
		await contentTabItem.ShowImageInfo(imageInfoView);
	}

	private async void OnImageEditRequested(
		object? sender, ImageSelectedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var imageFile = e.ImageFile;

		var imageEditView = _imageEditViewFactory.GetImageEditView(
			contentTabItem, imageFile);

		imageEditView.ImageFileOverwritten +=
			OnImageEditViewImageFileOverwritten;
		imageEditView.FolderContentChanged +=
			OnImageEditViewFolderContentChanged;

		await contentTabItem.ShowImageEdit(imageEditView);

		imageEditView.ImageFileOverwritten -=
			OnImageEditViewImageFileOverwritten;
		imageEditView.FolderContentChanged -=
			OnImageEditViewFolderContentChanged;

		contentTabItem.BringThumbnailIntoView();
	}

	private void OnCloneTabRequested(
		object? sender, ContentTabItemAddedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var fileSystemEntryInfoToClone = e.FileSystemEntryInfoToClone;
		var isExpandedFolderTreeViewSelectedItem =
			e.IsExpandedFolderTreeViewSelectedItem;

		_mainView.CloneContentTabItem(
			contentTabItem.TabOptions,
			fileSystemEntryInfoToClone,
			isExpandedFolderTreeViewSelectedItem);
	}

	private async void OnTabOptionsRequested(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		var tabOptionsView = _tabOptionsViewFactory.GetTabOptionsView(
			contentTabItem);

		tabOptionsView.TabOptionsChanged += OnTabOptionsChanged;
		await contentTabItem.ShowTabOptions(tabOptionsView);
		tabOptionsView.TabOptionsChanged -= OnTabOptionsChanged;
	}

	private async void OnThumbnailCacheOptionsRequested(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		var thumbnailCacheOptionsView = _thumbnailCacheOptionsViewFactory
			.GetThumbnailCacheOptionsView();

		thumbnailCacheOptionsView.EnableThumbnailCachingChanged +=
			OnEnableThumbnailCachingChanged;
		thumbnailCacheOptionsView.ClearThumbnailCacheSelected +=
			OnClearThumbnailCacheSelected;

		await contentTabItem.ShowThumbnailCacheOptions(
			thumbnailCacheOptionsView);

		thumbnailCacheOptionsView.EnableThumbnailCachingChanged -=
			OnEnableThumbnailCachingChanged;
		thumbnailCacheOptionsView.ClearThumbnailCacheSelected -=
			OnClearThumbnailCacheSelected;
	}

	private async void OnAboutInfoRequested(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		var aboutView = _aboutViewFactory.GetAboutView();
		await contentTabItem.ShowAboutInfo(aboutView);
	}

	private async void OnImageEditViewImageFileOverwritten(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		await contentTabItem.UpdateSelectedThumbnailAfterImageFileChange();
	}

	private void OnImageEditViewFolderContentChanged(
		object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;

		contentTabItem.RaiseFolderContentChangedEvent();
	}

	private static async void OnTabOptionsChanged(
		object? sender, TabOptionsChangedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var fileSystemEntryInfo = e.FileSystemEntryInfo;
		var tabOptions = e.TabOptions;
		var tabOptionChanges = e.TabOptionChanges;

		var hasChangedFolderTree =
			tabOptionChanges.HasChangedFolderOrdering ||
			(tabOptions.FolderOrdering !=
			 FileSystemEntryInfoOrdering.RandomShuffle &&
			 tabOptionChanges.HasChangedFolderOrderingDirection) ||
			tabOptionChanges.HasChangedZipArchivesEnabled;

		var hasChangedFolderContent =
			tabOptionChanges.HasChangedImageFileOrdering ||
			(tabOptions.ImageFileOrdering !=
			 FileSystemEntryInfoOrdering.RandomShuffle &&
			 tabOptionChanges.HasChangedImageFileOrderingDirection) ||
			tabOptionChanges.HasChangedThumbnailSize ||
			tabOptionChanges.HasChangedEnabledImageFileExtensions ||
			tabOptionChanges.HasChangedZipArchivesEnabled ||
			(tabOptionChanges.HasChangedRecursiveFolderBrowsing &&
			 fileSystemEntryInfo?.HasSubFolders == true) ||
			(tabOptions.RecursiveFolderBrowsing &&
			 fileSystemEntryInfo?.HasSubFolders == true &&
			 tabOptionChanges
				 .HasChangedGlobalOrderingForRecursiveFolderBrowsing) ||
			tabOptionChanges.HasChangedApplyImageOrientation ||
			tabOptionChanges.HasChangedShowThumbnailImageFileName;

		var hasChangedFolderInfo =
			tabOptionChanges.HasChangedRecursiveFolderBrowsing &&
			fileSystemEntryInfo?.HasSubFolders == false;

		var hasChangedPanelsSplittingRatio =
			tabOptionChanges.HasChangedPanelsSplittingRatio;

		var shouldSaveAsDefault = tabOptionChanges.ShouldSaveAsDefault;

		contentTabItem.RaiseContentTabItemChangedEvent(
			hasChangedFolderTree,
			hasChangedFolderContent,
			hasChangedFolderInfo,
			hasChangedPanelsSplittingRatio);

		if (shouldSaveAsDefault)
		{
			await contentTabItem.TabOptions!.SaveDefaultTabOptions();
		}
	}

	private async void OnEnableThumbnailCachingChanged(
		object? sender, EnableThumbnailCachingEventArgs e)
	{
		var thumbnailCacheOptions = e.ThumbnailCacheOptions;

		if (thumbnailCacheOptions.EnableThumbnailCaching)
		{
			_imageFileFactory.EnableThumbnailCaching();
		}
		else
		{
			_imageFileFactory.DisableThumbnailCaching();
		}

		await thumbnailCacheOptions.SaveThumbnailCacheOptions();
	}

	private async void OnClearThumbnailCacheSelected(
		object? sender, ClearThumbnailCacheEventArgs e)
	{
		var thumbnailCacheOptionsView = e.ThumbnailCacheOptionsView;

		await _databaseLogic.ClearDatabase();

		thumbnailCacheOptionsView.ThumbnailCacheSizeInMegabytes =
			_databaseLogic.GetThumbnailCacheSizeInMegabytes();
	}

	private async void OnContentTabItemChanged(
		object? sender, ContentTabItemChangedEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		var fileSystemEntryInfo = e.FileSystemEntryInfo;

		var hasChangedFolderTree = e.HasChangedFolderTree;
		var hasChangedFolderContent = e.HasChangedFolderContent;
		var hasChangedFolderInfo = e.HasChangedFolderInfo;
		var hasChangedPanelsSplittingRatio = e.HasChangedPanelsSplittingRatio;

		if (hasChangedFolderTree)
		{
			await ApplyFolderTreeChanges(contentTabItem, fileSystemEntryInfo);
		}

		if (hasChangedFolderContent)
		{
			await ApplyFolderContentChanges(
				contentTabItem, fileSystemEntryInfo);
		}

		if (hasChangedFolderInfo)
		{
			ApplyFolderInfoChanges(contentTabItem);
		}

		if (hasChangedPanelsSplittingRatio)
		{
			ApplyPanelsSplittingRatioChanges(contentTabItem);
		}
	}

	private async Task ApplyFolderTreeChanges(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo)
	{
		var isExpandedFolderTreeViewSelectedItem = contentTabItem
			.GetIsExpandedFolderTreeViewSelectedItem();

		DisableContentTabEventHandling(contentTabItem);

		await PopulateRootFolders(contentTabItem);

		await BuildFolderTreeViewFromActiveTab(
			contentTabItem,
			fileSystemEntryInfo,
			isExpandedFolderTreeViewSelectedItem);

		EnableContentTabEventHandling(contentTabItem);

		contentTabItem.SetFocusOnSelectedFolderTreeViewItem();
	}

	private async Task ApplyFolderContentChanges(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo)
	{
		var previousFolderVisualState = contentTabItem.FolderVisualState;
		previousFolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory
			.GetFolderVisualState(contentTabItem, fileSystemEntryInfo);

		await contentTabItem.FolderVisualState.UpdateVisualState(
			contentTabItem.TabOptions!);

		previousFolderVisualState?.DisposeCancellationTokenSource();
	}

	private static void ApplyFolderInfoChanges(
		IContentTabItem contentTabItem)
	{
		var folderVisualState = contentTabItem.FolderVisualState;
		folderVisualState?.SetFolderInfoText(contentTabItem.TabOptions!);

		contentTabItem.UpdateSelectedImageStatus();
	}

	private static void ApplyPanelsSplittingRatioChanges(
		IContentTabItem contentTabItem)
	{
		contentTabItem.UpdatePanelsSplittingRatio();
	}

	private async Task<IReadOnlyList<IFileSystemEntryInfo>> PopulateRootFolders(
		IContentTabItem contentTabItem)
	{
		await _discQueryEngine.BuildSkipRecursionFolderPaths();
		var rootFolders = await _discQueryEngine.GetRootFolders(
			contentTabItem.TabOptions!);

		contentTabItem.PopulateRootNodesSubFoldersTree(rootFolders);

		return rootFolders;
	}

	private async Task BuildFolderTreeViewFromInputPath(
		IContentTabItem contentTabItem,
		IInputPathHandler inputPathHandler,
		IReadOnlyList<IFileSystemEntryInfo> rootFolders,
		bool isExpandedFolderTreeViewSelectedItem)
	{
		IFileSystemEntryInfo? matchingFileSystemEntryInfo;
		var startAtRootFolders = true;

		var subFolders = rootFolders;

		do
		{
			matchingFileSystemEntryInfo = await inputPathHandler
				.GetMatchingFileSystemEntryInfo(subFolders);

			if (matchingFileSystemEntryInfo is not null)
			{
				contentTabItem.SaveMatchingTreeViewItem(
					matchingFileSystemEntryInfo.QualifiedPath,
					startAtRootFolders);

				subFolders = await _discQueryEngine.GetSubFolders(
					matchingFileSystemEntryInfo,
					contentTabItem.TabOptions!);

				contentTabItem.PopulateSubFoldersTreeOfParentTreeViewItem(
					subFolders);

				if (startAtRootFolders)
				{
					startAtRootFolders = false;
				}
			}
		} while (matchingFileSystemEntryInfo is not null);

		contentTabItem.SetIsExpandedFolderTreeViewSelectedItem(
			isExpandedFolderTreeViewSelectedItem);
	}

	private async Task BuildFolderTreeViewFromActiveTab(
		IContentTabItem contentTabItem,
		IFileSystemEntryInfo fileSystemEntryInfo,
		bool isExpandedFolderTreeViewSelectedItem)
	{
		var parentTreeFileSystemEntryInfoList =
			GetParentTreeFileSystemEntryInfoList(fileSystemEntryInfo);
		var startAtRootFolders = true;

		foreach (var aParentTreeFileSystemEntryInfo in
		         parentTreeFileSystemEntryInfoList)
		{
			contentTabItem.SaveMatchingTreeViewItem(
				aParentTreeFileSystemEntryInfo.QualifiedPath,
				startAtRootFolders);

			var subFolders = await _discQueryEngine.GetSubFolders(
				aParentTreeFileSystemEntryInfo,
				contentTabItem.TabOptions!);

			contentTabItem.PopulateSubFoldersTreeOfParentTreeViewItem(
				subFolders);

			if (startAtRootFolders)
			{
				startAtRootFolders = false;
			}
		}

		contentTabItem.SetIsExpandedFolderTreeViewSelectedItem(
			isExpandedFolderTreeViewSelectedItem);
	}

	private void EnableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.EnableFolderTreeViewSelectedItemChanged();

		contentTabItem.ContentTabItemChanged += OnContentTabItemChanged;

		contentTabItem.ImageInfoRequested += OnImageInfoRequested;
		contentTabItem.ImageEditRequested += OnImageEditRequested;

		contentTabItem.CloneTabRequested += OnCloneTabRequested;

		contentTabItem.TabOptionsRequested += OnTabOptionsRequested;
		contentTabItem.ThumbnailCacheOptionsRequested +=
			OnThumbnailCacheOptionsRequested;

		contentTabItem.AboutInfoRequested += OnAboutInfoRequested;
	}

	private void DisableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.DisableFolderTreeViewSelectedItemChanged();

		contentTabItem.ContentTabItemChanged -= OnContentTabItemChanged;

		contentTabItem.ImageInfoRequested -= OnImageInfoRequested;
		contentTabItem.ImageEditRequested -= OnImageEditRequested;

		contentTabItem.CloneTabRequested -= OnCloneTabRequested;

		contentTabItem.TabOptionsRequested -= OnTabOptionsRequested;
		contentTabItem.ThumbnailCacheOptionsRequested -=
			OnThumbnailCacheOptionsRequested;

		contentTabItem.AboutInfoRequested -= OnAboutInfoRequested;
	}

	private async Task ClearContentTabItem(IContentTabItem contentTabItem)
	{
		var folderVisualState = contentTabItem.FolderVisualState;
		await ClearFolderVisualState(folderVisualState);

		DisableContentTabEventHandling(contentTabItem);

		contentTabItem.DisposeFolderChangedMutex();
	}

	private static async Task ClearFolderVisualState(
		IFolderVisualState? folderVisualState)
	{
		if (folderVisualState is not null)
		{
			folderVisualState.NotifyStopThumbnailGeneration();
			await folderVisualState.ClearVisualState();

			folderVisualState.DisposeCancellationTokenSource();
		}
	}

	private static IReadOnlyList<IFileSystemEntryInfo>
		GetParentTreeFileSystemEntryInfoList(
			IFileSystemEntryInfo fileSystemEntryInfo)
	{
		var parentTreeFileSystemEntryInfoList =
			new List<IFileSystemEntryInfo>();

		for (var currentFileSystemEntryInfo = fileSystemEntryInfo;
		     currentFileSystemEntryInfo is not null;
		     currentFileSystemEntryInfo = currentFileSystemEntryInfo.Parent)
		{
			parentTreeFileSystemEntryInfoList.Add(currentFileSystemEntryInfo);
		}

		parentTreeFileSystemEntryInfoList.Reverse();
		return parentTreeFileSystemEntryInfoList;
	}
}
