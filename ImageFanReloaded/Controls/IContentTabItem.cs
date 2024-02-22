using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Controls;

public interface IContentTabItem
{
	TabItem? TabItem { get; set; }
	Window? Window { get; set; }
	
	IMainView? MainView { get; set; }
	IContentTabItemHeader? ContentTabItemHeader { get; set; }
	
    IImageViewFactory? ImageViewFactory { get; set; }
	object? GenerateThumbnailsLockObject { get; set; }
	
	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;
	
	void OnKeyDown(object? sender, KeyEventArgs e);

	void SetTitle(string title);

	void RegisterMainViewEvents();
	void UnregisterMainViewEvents();

	void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
								bool rootNodes);

	void ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);

	void SetFolderStatusBarText(string folderStatusBarText);
	void SetImageStatusBarText(string imageStatusBarText);
}
