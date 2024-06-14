using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualState
{
    void NotifyStopThumbnailGeneration();
    void ClearVisualState();

    Task UpdateVisualState(bool recursiveFolderAccess);
}
