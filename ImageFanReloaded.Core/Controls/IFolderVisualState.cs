namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualState
{
    void NotifyStopThumbnailGeneration();

    void UpdateVisualState();
    void ClearVisualState();
}
