using System;
using System.Collections.Generic;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IContentTabItem
{
	IMainView? MainView { get; set; }
	
	IGlobalParameters? GlobalParameters { get; set; }
	FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; }
	int ThumbnailSize { get; }
	
	IFolderChangedMutex? FolderChangedMutex { get; set; }

	object? WrapperTabItem { get; set; }
	IContentTabItemHeader? ContentTabItemHeader { get; set; }
	
    IImageViewFactory? ImageViewFactory { get; set; }
	
	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;
	event EventHandler<FolderOrderingChangedEventArgs>? FolderOrderingChanged;
	
	void EnableFolderTreeViewSelectedItemChanged();
	void DisableFolderTreeViewSelectedItemChanged();

	bool ShouldHandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing);
	void HandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing);
	
	void SetFolderTreeViewSelectedItem();

	bool? GetFolderTreeViewSelectedItemExpandedState();
	void SetFolderTreeViewSelectedItemExpandedState(bool isExpanded);

	void SetTitle(string title);

	void RegisterMainViewEvents();
	void UnregisterMainViewEvents();

	void PopulateRootNodesSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> rootFolders);
	void PopulateSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> subFolders);
	void PopulateSubFoldersTreeOfParentTreeViewItem(IReadOnlyList<FileSystemEntryInfo> subFolders);

	void ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection);

	void SetFolderStatusBarText(string folderStatusBarText);
	void SetImageStatusBarText(string imageStatusBarText);

	void SaveMatchingTreeViewItem(FileSystemEntryInfo selectedFileSystemEntryInfo, bool startAtRootFolders);

	bool AreFolderInfoOrImageInfoFocused();
	void FocusThumbnailScrollViewer();
}
