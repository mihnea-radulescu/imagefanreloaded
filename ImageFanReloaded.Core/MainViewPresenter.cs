using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core;

public class MainViewPresenter
{
	public MainViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IFolderVisualStateFactory folderVisualStateFactory,
		IImageViewFactory imageViewFactory,
		IInputPathContainer inputPathContainer,
		IGlobalParameters globalParameters,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;

		_inputPathContainer = inputPathContainer;
		_globalParameters = globalParameters;

		_mainView = mainView;
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;
		_mainView.HelpMenuRequested += OnHelpMenuRequested;
	}

	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;
	
	private readonly IInputPathContainer _inputPathContainer;
	private readonly IGlobalParameters _globalParameters;

	private readonly IMainView _mainView;

	private async void OnContentTabItemAdded(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		contentTabItem.ImageViewFactory = _imageViewFactory;

		var rootFolders = await PopulateRootFolders(contentTabItem);
		
		if (_inputPathContainer.ShouldProcessInputPath())
		{
			await PopulateInputPath(contentTabItem, rootFolders);
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
		
		contentTabItem.FolderChanged -= OnFolderChanged;
		ClearContentTabItem(contentTabItem);
		
		contentTabItem.FolderChangedMutex!.Dispose();
	}
	
	private async void OnHelpMenuRequested(object? sender, EventArgs e)
	{
		await _mainView.ShowInfoMessage(_globalParameters.AboutTitle, _globalParameters.AboutText);
	}
	
	private async void OnFolderChanged(object? sender, FolderChangedEventArgs e)
	{
		var contentTabItem = (IContentTabItem)sender!;
		
		var folderName = e.Name;
		var folderPath = e.Path;
		var thumbnailSize = e.ThumbnailSize;
		var recursive = e.Recursive;
		
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			folderName,
			folderPath);

		await contentTabItem.FolderVisualState.UpdateVisualState(thumbnailSize, recursive);
	}

	private async Task<IReadOnlyList<FileSystemEntryInfo>> PopulateRootFolders(IContentTabItem contentTabItem)
	{
		var rootFolders = await _discQueryEngine.GetRootFolders();
		
		contentTabItem.PopulateRootNodesSubFoldersTree(rootFolders);

		return rootFolders;
	}
	
	private static void ClearContentTabItem(IContentTabItem contentTabItem)
	{
		contentTabItem.FolderVisualState?.NotifyStopThumbnailGeneration();

		contentTabItem.FolderVisualState?.ClearVisualState();
	}

	private async Task PopulateInputPath(IContentTabItem contentTabItem, IReadOnlyList<FileSystemEntryInfo> rootFolders)
	{
		_inputPathContainer.DisableProcessInputPath();

		await BuildInputFolderTreeView(contentTabItem, rootFolders);

		EnableContentTabEventHandling(contentTabItem);

		await RenderInputFolderImages(contentTabItem);
	}
	
	private async Task BuildInputFolderTreeView(
		IContentTabItem contentTabItem, IReadOnlyList<FileSystemEntryInfo> rootFolders)
	{
		FileSystemEntryInfo? matchingFileSystemEntryInfo;
		var subFolders = rootFolders;

		do
		{
			matchingFileSystemEntryInfo = await _inputPathContainer.GetMatchingFileSystemEntryInfo(subFolders);

			if (matchingFileSystemEntryInfo is not null)
			{
				contentTabItem.SaveMatchingTreeViewItem(matchingFileSystemEntryInfo);
				
				subFolders = await _discQueryEngine.GetSubFolders(matchingFileSystemEntryInfo.Path);
				contentTabItem.PopulateSubFoldersTreeOfParentTreeViewItem(subFolders);
			}
		} while (matchingFileSystemEntryInfo is not null);
	}

	private void EnableContentTabEventHandling(IContentTabItem contentTabItem)
	{
		contentTabItem.EnableFolderTreeViewSelectedItemChanged();
		
		contentTabItem.FolderChanged += OnFolderChanged;
	}
	
	private async Task RenderInputFolderImages(IContentTabItem contentTabItem)
	{
		var fileSystemEntryInfo = await _inputPathContainer.GetFileSystemEntryInfo();
		
		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			fileSystemEntryInfo.Name,
			fileSystemEntryInfo.Path);

		await contentTabItem.FolderVisualState.UpdateVisualState(_globalParameters.DefaultThumbnailSize, false);
	}

	#endregion
}
