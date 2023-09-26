﻿using System;
using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Controls;

public interface IContentTabItem
{
	IImageViewFactory ImageViewFactory { get; set; }
	object GenerateThumbnailsLockObject { get; set; }

	string Title { get; set; }

	event EventHandler<FolderChangedEventArgs> FolderChanged;

	IFolderVisualState FolderVisualState { get; set; }

	void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
								bool rootNodes);

	void ClearThumbnailBoxes();
	void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
	void RefreshThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
}