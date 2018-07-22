using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Views.Interface
{
    public interface IMainView
    {
        event EventHandler<FolderChangedEventArgs> FolderChanged;

        void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
                                    bool rootNodes);

        void ClearThumbnailBoxes();
        void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnails);

        void Show();
    }
}
