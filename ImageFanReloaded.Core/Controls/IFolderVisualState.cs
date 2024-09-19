using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualState
{
    void NotifyStopThumbnailGeneration();
    void ClearVisualState();

    Task UpdateVisualState(
        FileSystemEntryInfoOrdering fileSystemEntryInfoOrdering, int thumbnailSize, bool recursiveFolderAccess);
}
