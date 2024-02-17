using System;
using System.Collections.Generic;
using Avalonia.Input;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Controls;

public interface IContentTabItem
{
	IImageViewFactory? ImageViewFactory { get; set; }
	object? GenerateThumbnailsLockObject { get; set; }

	string Title { get; set; }
	
	IFolderVisualState? FolderVisualState { get; set; }

	event EventHandler<FolderChangedEventArgs>? FolderChanged;
	
	void OnKeyPressed(object? sender, KeyEventArgs e);

	void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
								bool rootNodes);

	void ClearThumbnailBoxes();
	void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
}
