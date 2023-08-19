using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Infrastructure.Implementation;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Factories.Implementation
{
    public class FolderVisualStateFactory
        : IFolderVisualStateFactory
    {
        public IFolderVisualState GetFolderVisualState(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            object generateThumbnailsLockObject,
            string folderPath)
        {
            IFolderVisualState folderVisualState = new FolderVisualState(
                discQueryEngine,
                mainView,
                dispatcher,
                generateThumbnailsLockObject,
                folderPath);

            return folderVisualState;
        }
    }
}
