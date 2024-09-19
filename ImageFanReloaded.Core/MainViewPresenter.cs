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
		IInputPathContainer inputPathContainer,
		IAboutViewFactory aboutViewFactory,
		IMainView mainView)
	{
		_discQueryEngine = discQueryEngine;
		_folderVisualStateFactory = folderVisualStateFactory;
		_imageViewFactory = imageViewFactory;

		_inputPathContainer = inputPathContainer;
		_aboutViewFactory = aboutViewFactory;

		_mainView = mainView;
		_mainView.ContentTabItemAdded += OnContentTabItemAdded;
		_mainView.ContentTabItemClosed += OnContentTabItemClosed;
		_mainView.AboutInfoRequested += OnAboutInfoRequested;
	}

	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IFolderVisualStateFactory _folderVisualStateFactory;
	private readonly IImageViewFactory _imageViewFactory;
	
	private readonly IInputPathContainer _inputPathContainer;
	private readonly IAboutViewFactory _aboutViewFactory;

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
	}
	
	private async Task RenderInputFolderImages(IContentTabItem contentTabItem)
	{
		var fileSystemEntryInfo = await _inputPathContainer.GetFileSystemEntryInfo();
		
		contentTabItem.FolderVisualState = _folderVisualStateFactory.GetFolderVisualState(
			contentTabItem,
			fileSystemEntryInfo.Name,
			fileSystemEntryInfo.Path);

		await contentTabItem.FolderVisualState.UpdateVisualState(
			contentTabItem.FileSystemEntryInfoOrdering, contentTabItem.ThumbnailSize, false);
	}

	#endregion
}
