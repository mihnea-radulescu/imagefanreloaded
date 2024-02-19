namespace ImageFanReloaded.Infrastructure;

public interface IFolderVisualState
{
    void NotifyStopThumbnailGeneration();

    void UpdateVisualState();
    void ClearVisualState();
}
