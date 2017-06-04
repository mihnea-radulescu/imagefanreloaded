using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Views.Interface
{
    public interface IMainView
    {
        event EventHandler<FileSystemEntryChangedEventArgs> FolderChanged;

        void PopulateSubFoldersTree(IEnumerable<FileSystemEntryInfo> subFolders,
                                    bool rootNodes);

        void ClearThumbnailBoxes();
        void PopulateThumbnailBoxes(IEnumerable<ThumbnailInfo> thumbnails);

        void Show();
    }
}
