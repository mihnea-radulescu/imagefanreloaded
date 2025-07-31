using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IContentTabItem
{
	IMainView? MainView { get; set; }

	IGlobalParameters? GlobalParameters { get; set; }
	IMouseCursorFactory? MouseCursorFactory { get; set; }

	ITabOptions? TabOptions { get; set; }

	IAsyncMutex? FolderChangedMutex { get; set; }
	void DisposeFolderChangedMutex();

	object? WrapperTabItem { get; set; }
	IContentTabItemHeader? ContentTabItemHeader { get; set; }

	IImageViewFactory? ImageViewFactory { get; set; }

	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;
	event EventHandler<FolderOrderingChangedEventArgs>? FolderOrderingChanged;

	event EventHandler<ImageSelectedEventArgs>? ImageInfoRequested;
	event EventHandler<ImageSelectedEventArgs>? ImageEditRequested;
	event EventHandler<ContentTabItemEventArgs>? TabOptionsRequested;
	event EventHandler<ContentTabItemEventArgs>? AboutInfoRequested;

	void EnableFolderTreeViewSelectedItemChanged();
	void DisableFolderTreeViewSelectedItemChanged();

	bool ShouldHandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing);
	void HandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing);

	void SetFocusOnSelectedFolderTreeViewItem();

	bool? GetFolderTreeViewSelectedItemExpandedState();
	void SetFolderTreeViewSelectedItemExpandedState(bool isExpanded);

	void SetTabInfo(string folderName, string folderPath);

	void RegisterMainViewEvents();
	void UnregisterMainViewEvents();

	void PopulateRootNodesSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> rootFolders);
	void PopulateSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> subFolders);
	void PopulateSubFoldersTreeOfParentTreeViewItem(IReadOnlyList<FileSystemEntryInfo> subFolders);

	Task ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection);

	void SetFolderInfoText(string folderInfoText);
	void SetImageInfoText(string imageInfoText);

	void SaveMatchingTreeViewItem(FileSystemEntryInfo selectedFileSystemEntryInfo, bool startAtRootFolders);

	bool AreFolderInfoOrImageInfoFocused();
	void FocusThumbnailScrollViewer();

	void RaiseFolderChangedEvent();
	void RaiseFolderOrderingChangedEvent();
	void RaisePanelsSplittingRatioChangedEvent();

	Task RefreshSelectedImage();
	Task UpdateSelectedImageAfterImageFileChange();

	Task ShowImageEdit(IImageEditView imageEditView);
	Task ShowTabOptions(ITabOptionsView tabOptionsView);
	Task ShowAboutInfo(IAboutView aboutView);
	Task ShowImageInfo(IImageInfoView imageInfoView);
}
