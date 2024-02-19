using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Controls;

public interface IContentTabItem
{
	TabItem? TabItem { get; set; }
	Window? Window { get; set; }
	
	IContentTabItemHeader? ContentTabItemHeader { get; set; }
	
    IImageViewFactory? ImageViewFactory { get; set; }
	object? GenerateThumbnailsLockObject { get; set; }
	
	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;
	
	void OnKeyPressed(object? sender, KeyEventArgs e);
	void OnTabCountChanged(object? sender, TabCountChangedEventArgs e);

	void SetTitle(string title);

	void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
								bool rootNodes);

	void ClearThumbnailBoxes(bool resetContent);
	void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);

	void SetStatusBarText(string statusBarText);
}
