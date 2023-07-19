using System;
using System.Collections.Generic;

using ImageFanReloadedWPF.CommonTypes.CommonEventArgs;
using ImageFanReloadedWPF.CommonTypes.Info;

namespace ImageFanReloadedWPF.Views.Interface
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
