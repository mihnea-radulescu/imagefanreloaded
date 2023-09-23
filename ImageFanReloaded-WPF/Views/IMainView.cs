using System;
using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Views
{
    public interface IMainView
    {
        event EventHandler<FolderChangedEventArgs> FolderChanged;

        void PopulateSubFoldersTree(ICollection<FileSystemEntryInfo> subFolders,
                                    bool rootNodes);

        void ClearThumbnailBoxes();
        void PopulateThumbnailBoxes(ICollection<ThumbnailInfo> thumbnailInfoCollection);
        void RefreshThumbnailBoxes(ICollection<ThumbnailInfo> thumbnailInfoCollection);
    }
}
