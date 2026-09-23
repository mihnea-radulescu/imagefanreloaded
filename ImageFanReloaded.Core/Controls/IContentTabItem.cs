using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
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

	event EventHandler<ContentTabItemChangedEventArgs>? ContentTabItemChanged;

	event EventHandler<ImageSelectedEventArgs>? ImageInfoRequested;
	event EventHandler<ImageSelectedEventArgs>? ImageEditRequested;

	event EventHandler<ContentTabItemAddedEventArgs>? CloneTabRequested;

	event EventHandler<ContentTabItemEventArgs>? TabOptionsRequested;
	event EventHandler<ContentTabItemEventArgs>? ThumbnailCacheOptionsRequested;

	event EventHandler<ContentTabItemEventArgs>? AboutInfoRequested;

	void EnableFolderTreeViewSelectedItemChanged();
	void DisableFolderTreeViewSelectedItemChanged();

	bool ShouldHandleContentTabItemKeyFunctions(
		KeyModifiers keyModifiers, Key keyPressing);
	void HandleContentTabItemKeyFunctions(
		KeyModifiers keyModifiers, Key keyPressing);

	void SetFocusOnSelectedFolderTreeViewItem();

	bool GetIsExpandedFolderTreeViewSelectedItem();
	void SetIsExpandedFolderTreeViewSelectedItem(bool isExpanded);

	void SetTabInfo(string folderName, string folderPath);

	void RegisterMainViewEvents();
	void UnregisterMainViewEvents();

	void PopulateRootNodesSubFoldersTree(
		IReadOnlyList<IFileSystemEntryInfo> rootFolders);
	void PopulateSubFoldersTree(IReadOnlyList<IFileSystemEntryInfo> subFolders);
	void PopulateSubFoldersTreeOfParentTreeViewItem(
		IReadOnlyList<IFileSystemEntryInfo> subFolders);

	Task ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(
		IReadOnlyList<IThumbnailInfo> thumbnailInfoList);
	void RefreshThumbnailBoxes(
		IReadOnlyList<IThumbnailInfo> thumbnailInfoList);

	IFileSystemEntryInfo? GetActiveFileSystemEntryInfo();

	void SetFolderInfoText(string folderInfoText);
	void SetImageInfoText(string imageInfoText);

	void SaveMatchingTreeViewItem(string path, bool startAtRootFolders);

	bool AreSelectedFolderInfoTextOrImageInfoText { get; }

	void FocusThumbnailScrollViewer();
	void BringThumbnailIntoView();

	void RaiseContentTabItemChangedEvent(
		bool hasChangedFolderTree,
		bool hasChangedFolderContent,
		bool hasChangedFolderInfo,
		bool hasChangedPanelsSplittingRatio);

	void RaiseFolderTreeChangedEvent();
	void RaiseFolderContentChangedEvent();
	void RaiseFolderInfoChangedEvent();
	void RaisePanelsSplittingRatioChangedEvent();

	void UpdatePanelsSplittingRatio();
	void UpdateSelectedImageStatus();
	Task UpdateSelectedThumbnailAfterImageFileChange();

	Task ShowImageInfo(IImageInfoView imageInfoView);
	Task ShowImageEdit(IImageEditView imageEditView);

	Task ShowTabOptions(ITabOptionsView tabOptionsView);
	Task ShowThumbnailCacheOptions(
		IThumbnailCacheOptionsView thumbnailCacheOptionsView);

	Task ShowAboutInfo(IAboutView aboutView);
}
