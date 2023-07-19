namespace ImageFanReloaded.Infrastructure.Interface
{
    public interface IFolderVisualState
    {
        void NotifyStopThumbnailGeneration();

        void UpdateVisualState();
    }
}
