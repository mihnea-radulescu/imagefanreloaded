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
        void PopulateThumbnailBoxes(ICollection<ThumbnailInfo> thumbnails);
        void RefreshThumbnailBoxes(ICollection<ThumbnailInfo> thumbnails);

        void Show();
    }
}
