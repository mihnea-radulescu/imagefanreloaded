using System;
using System.Collections.Generic;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Views;

public interface IMainView
{
    event EventHandler<FolderChangedEventArgs> FolderChanged;

    void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
                                bool rootNodes);

    void ClearThumbnailBoxes();
    void PopulateThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
    void RefreshThumbnailBoxes(IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection);
}
