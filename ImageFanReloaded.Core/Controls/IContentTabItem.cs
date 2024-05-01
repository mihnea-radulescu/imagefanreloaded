using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IContentTabItem
{
	IMainView? MainView { get; set; }
	IGlobalParameters? GlobalParameters { get; set; }
	IFolderChangedEventHandle? FolderChangedEventHandle { get; set; }

	object? WrapperTabItem { get; set; }
	IContentTabItemHeader? ContentTabItemHeader { get; set; }
	
    IImageViewFactory? ImageViewFactory { get; set; }
	
	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;

	void EnableFolderTreeViewSelectedItemChanged();
	
	void OnKeyPressed(object? sender, KeyboardKeyEventArgs e);

	void SetTitle(string title);

	void RegisterMainViewEvents();
	void UnregisterMainViewEvents();

	void PopulateRootNodesSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> rootFolders);
	void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders);
	void PopulateSubFoldersTreeOfParentTreeViewItem(IReadOnlyCollection<FileSystemEntryInfo> subFolders);

	void ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(IReadOnlyCollection<IThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyCollection<IThumbnailInfo> thumbnailInfoCollection);

	void SetFolderStatusBarText(string folderStatusBarText);
	void SetImageStatusBarText(string imageStatusBarText);

	void SaveMatchingTreeViewItem(FileSystemEntryInfo selectedFileSystemEntryInfo);
}
