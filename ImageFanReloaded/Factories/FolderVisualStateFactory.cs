using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Infrastructure.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories
{
    public class FolderVisualStateFactory : IFolderVisualStateFactory
    {
        public IFolderVisualState GetFolderVisualState(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            object generateThumbnailsLockObject,
            string folderPath)
        {
            var folderVisualState = new FolderVisualState(
                discQueryEngine,
                mainView,
                dispatcher,
                generateThumbnailsLockObject,
                folderPath);

            return folderVisualState;
        }
    }
}
