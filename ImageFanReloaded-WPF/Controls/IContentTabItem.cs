using System;
using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Controls
{
	public interface IContentTabItem
	{
		IImageViewFactory ImageViewFactory { get; set; }
		object GenerateThumbnailsLockObject { get; set; }

		string Title { get; set; }

		event EventHandler<FolderChangedEventArgs> FolderChanged;

		IFolderVisualState FolderVisualState { get; set; }

		void PopulateSubFoldersTree(ICollection<FileSystemEntryInfo> subFolders,
									bool rootNodes);

		void ClearThumbnailBoxes();
		void PopulateThumbnailBoxes(ICollection<ThumbnailInfo> thumbnailInfoCollection);
		void RefreshThumbnailBoxes(ICollection<ThumbnailInfo> thumbnailInfoCollection);
	}
}
