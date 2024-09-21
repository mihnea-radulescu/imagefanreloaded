using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;

namespace ImageFanReloaded.Core;

public class MainViewPresenter
{
	public MainViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IFolderVisualStateFactory folderVisualStateFactory,
		IImageViewFactory imageViewFactory,
		IAboutViewFactory aboutViewFactory,
		IInputPathHandlerFactory inputPathHandlerFactory,
		IInputPathHandler commandLineArgsInputPathHandler,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;
		_aboutViewFactory = aboutViewFactory;
		_inputPathHandlerFactory = inputPathHandlerFactory;
		
		_commandLineArgsInputPathHandler = commandLineArgsInputPathHandler;
		_shouldProcessCommandLineArgsInputPath = true;

		_mainView = mainView;
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;
		_mainView.AboutInfoRequested += OnAboutInfoRequested;
	}

	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;
	private readonly IAboutViewFactory _aboutViewFactory;
	private readonly IInputPathHandlerFactory _inputPathHandlerFactory;
	
	private readonly IInputPathHandler _commandLineArgsInputPathHandler;
	private bool _shouldProcessCommandLineArgsInputPath;

	private readonly IMainView _mainView;

	private async void OnContentTabItemAdded(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		contentTabItem.ImageViewFactory = _imageViewFactory;

		var rootFolders = await PopulateRootFolders(contentTabItem);
		
		if (_shouldProcessCommandLineArgsInputPath && _commandLineArgsInputPathHandler.CanProcessInputPath())
		{
			await PopulateInputPath(contentTabItem, rootFolders, _commandLineArgsInputPathHandler);

			_shouldProcessCommandLineArgsInputPath = false;
		}
		else
		{
			EnableContentTabEventHandling(contentTabItem);
		}
		
		contentTabItem.SetFolderTreeViewSelectedItem();
	}
	
	private void OnContentTabItemClosed(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		
		ClearContentTabItem(contentTabItem);
		
		contentTabItem.FolderChangedMutex!.Dispose();
	}
	
	private async void OnAboutInfoRequested(object? sender, EventArgs e)
	{
		var aboutView = _aboutViewFactory.GetAboutView();
		await _mainView.ShowAboutInfo(aboutView);
	}
	
	private async void OnFolderChanged(object? sender, FolderChangedEventArgs e)
	{
		var contentTabItem = (IContentTabItem)sender!;
		
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			e.Name,
			e.Path);

		await contentTabItem.FolderVisualState.UpdateVisualState(
			e.FileSystemEntryInfoOrdering, e.ThumbnailSize, e.Recursive);
	}
	
	private async void OnFolderOrderingChanged(object? sender, FolderChangedEventArgs e)
	{
		var contentTabItem = (IContentTabItem)sender!;
		
		var folderPath = contentTabItem.GetFolderTreeViewSelectedItemFolderPath();
		var isExpandedFolderTreeViewSelectedItem = contentTabItem.GetFolderTreeViewSelectedItemExpandedState()
			?? false;

		ClearContentTabItem(contentTabItem);

		var rootFolders = await PopulateRootFolders(contentTabItem);

		var folderChangedInputPathHandler = _inputPathHandlerFactory.GetInputPathHandler(folderPath);
		await PopulateInputPath(contentTabItem, rootFolders, folderChangedInputPathHandler);

		contentTabItem.SetFolderTreeViewSelectedItem();
		contentTabItem.SetFolderTreeViewSelectedItemExpandedState(isExpandedFolderTreeViewSelectedItem);
	}

	private async Task<IReadOnlyList<FileSystemEntryInfo>> PopulateRootFolders(IContentTabItem contentTabItem)
	{
		var rootFolders = await _discQueryEngine.GetRootFolders();
		
		contentTabItem.PopulateRootNodesSubFoldersTree(rootFolders);

		return rootFolders;
	}
	
	private void ClearContentTabItem(IContentTabItem contentTabItem)
	{
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();
		contentTabItem.FolderVisualState?.ClearVisualState();
		
		DisableContentTabEventHandling(contentTabItem);
	}

	private async Task PopulateInputPath(
		IContentTabItem contentTabItem,
		IReadOnlyList<FileSystemEntryInfo> rootFolders,
		IInputPathHandler inputPathHandler)
	{
		await BuildInputFolderTreeView(contentTabItem, rootFolders, inputPathHandler);

		EnableContentTabEventHandling(contentTabItem);

		await RenderInputFolderImages(contentTabItem, inputPathHandler);
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
					matchingFileSystemEntryInfo.Path, contentTabItem.FileSystemEntryInfoOrdering);
				contentTabItem.PopulateSubFoldersTreeOfParentTreeViewItem(subFolders);
			}
		} while (matchingFileSystemEntryInfo is not null);
	}

	private void EnableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.EnableFolderTreeViewSelectedItemChanged();
		
		contentTabItem.FolderChanged += OnFolderChanged;
		contentTabItem.FolderOrderingChanged += OnFolderOrderingChanged;
	}
	
	private void DisableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.DisableFolderTreeViewSelectedItemChanged();
		
		contentTabItem.FolderChanged -= OnFolderChanged;
		contentTabItem.FolderOrderingChanged -= OnFolderOrderingChanged;
	}
	
	private async Task RenderInputFolderImages(IContentTabItem contentTabItem, IInputPathHandler inputPathHandler)
	{
		var fileSystemEntryInfo = await inputPathHandler.GetFileSystemEntryInfo();
		
		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			fileSystemEntryInfo.Name,
			fileSystemEntryInfo.Path);

		await contentTabItem.FolderVisualState.UpdateVisualState(
			contentTabItem.FileSystemEntryInfoOrdering, contentTabItem.ThumbnailSize, false);
	}

	#endregion
}
